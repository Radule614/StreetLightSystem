import { produce } from "immer";
import { Member, Team, extractErrorMessages, noCachedDataWarn, notifyErrors } from "../../../shared";
import { AppState, GetAppState, Request, SetAppState, apiUrl } from "../store";
import axios from "axios";
import { toast } from "react-toastify";

export interface TeamDto {
  name: string;
  memberIds: string[];
}

export interface TeamStoreType {
  teams: Request<Team[]>;
  members: Request<Member[]>;
  teamData: Request<Team>;
  fetchTeams: () => Promise<void>;
  fetchMembers: () => Promise<void>;
  fetchTeamData: (teamId: string) => Promise<void>
  createTeam: (team: TeamDto) => Promise<void>
  updateTeam: (teamId: string, team: TeamDto) => Promise<void>;
  deleteTeam: (teamId: string) => Promise<void>
  clearData: () => void;
}

export const teamStore = (
  set: SetAppState,
  get: GetAppState
): TeamStoreType => ({
  teams: {
    data: null,
    isLoading: false,
    errors: []
  },
  members: {
    data: null,
    isLoading: false,
    errors: []
  },
  teamData: {
    data: null,
    isLoading: false,
    errors: []
  },
  fetchTeams: async () => {
    get().team.teams.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.team.teams.isLoading = true;
        draft.team.teams.data = null;
        draft.team.teams.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/team`, {
        signal: get().team.teams.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.team.teams.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      if (error.code === "ERR_NETWORK")
        noCachedDataWarn("Teams");
      else
        notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.team.teams.isLoading = false;
        return draft;
      })
    )
  },
  fetchMembers: async () => {
    get().team.members.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.team.members.isLoading = true;
        draft.team.members.data = null;
        draft.team.members.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/team/members`, {
        signal: get().team.members.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.team.members.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      if (error.code === "ERR_NETWORK")
        noCachedDataWarn("Team members");
      else
        notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.team.members.isLoading = false;
        return draft;
      })
    )
  },
  fetchTeamData: async (teamId: string) => {
    get().team.teamData.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.team.teamData.data = null;
        draft.team.teamData.isLoading = true;
        draft.team.teamData.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/team/${teamId}`, {
        signal: get().team.teamData.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.team.teamData.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      if (error.code === "ERR_NETWORK")
        noCachedDataWarn("Team data");
      else
        notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.team.teamData.isLoading = false;
        return draft;
      })
    )
  },
  createTeam: async (team: TeamDto) => {
    try {
      await axios.post(`${apiUrl}/team`, team)
      toast.success("Team created successfully.")
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  updateTeam: async (teamId: string, team: TeamDto) => {
    try {
      await axios.put(`${apiUrl}/team`, { ...team, id: teamId })
      toast.success("Team updated successfully.")
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  deleteTeam: async (teamId: string) => {
    try {
      await axios.delete(`${apiUrl}/team/${teamId}`)
      toast.success("Team deleted successfully.")
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  clearData: () => {
    set(
      produce((draft: AppState) => {
        draft.team.teams.data = null;
        draft.team.teams.isLoading = true;
        return draft;
      })
    )
  }
})