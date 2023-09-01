import { produce } from "immer";
import { Repair, extractErrorMessages, noCachedDataWarn, notifyErrors, requestCachedWarning } from "../../../shared";
import { SetAppState, GetAppState, Request, AppState, apiUrl } from "../store";
import axios from "axios";
import { toast } from "react-toastify";

export interface RepairStoreType {
  teamHistory: Request<Repair[]>;
  poleHistory: Request<Repair[]>;
  fetchPoleHistory: (poleId: string) => Promise<void>
  fetchTeamHistory: (teamId: string) => Promise<void>
  startRepairProcess: (poleId: string) => Promise<void>
  endRepairProcess: (repairId: string, success: boolean) => Promise<void>
  clearData: () => void;
}

export const repairStore = (
  set: SetAppState,
  get: GetAppState
): RepairStoreType => ({
  teamHistory: {
    data: null,
    isLoading: false,
    errors: []
  },
  poleHistory: {
    data: null,
    isLoading: false,
    errors: []
  },
  fetchTeamHistory: async (teamId: string) => {
    if (teamId === "") return;
    get().repair.teamHistory.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.repair.teamHistory.data = null;
        draft.repair.teamHistory.isLoading = true;
        draft.repair.teamHistory.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/repair/history/team/${teamId}`, {
        signal: get().repair.teamHistory.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.repair.teamHistory.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      if (error.code === "ERR_NETWORK")
        noCachedDataWarn("Team repair history");
      else
        notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.repair.teamHistory.isLoading = false;
        return draft;
      })
    )
  },
  fetchPoleHistory: async (poleId: string) => {
    if (poleId === "") return;
    get().repair.poleHistory.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.repair.poleHistory.data = null;
        draft.repair.poleHistory.isLoading = true;
        draft.repair.poleHistory.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/repair/history/pole/${poleId}`, {
        signal: get().repair.poleHistory.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.repair.poleHistory.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      if (error.code === "ERR_NETWORK")
        noCachedDataWarn("Pole repair history");
      else
        notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.repair.poleHistory.isLoading = false;
        return draft;
      })
    )
  },
  startRepairProcess: async (poleId: string) => {
    try {
      await axios.post(`${apiUrl}/repair/start`, { poleId })
      toast.info("Repair data sent.")
    } catch (error: any) {
      if (error.code === "ERR_NETWORK")
        requestCachedWarning("Start repair process");
      else
        notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  endRepairProcess: async (repairId: string, success: boolean) => {
    try {
      await axios.put(`${apiUrl}/repair/end`, { repairId, success })
      toast.info("Repair data sent.")
    } catch (error: any) {
      if (error.code === "ERR_NETWORK")
        requestCachedWarning("End repair process");
      else
        notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  clearData: () => {
    set(
      produce((draft: AppState) => {
        draft.repair.poleHistory.data = null;
        draft.repair.poleHistory.isLoading = true;
        draft.repair.teamHistory.data = null;
        draft.repair.teamHistory.isLoading = true;
        return draft;
      })
    )
  }
})