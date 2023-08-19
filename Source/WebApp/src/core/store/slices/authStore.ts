import { produce } from "immer";
import { SetAppState, GetAppState, Request, AppState, apiUrl } from "../store";
import axios from "axios";
import { Team, User, extractErrorMessages, notifyErrors } from "../../../shared";
import { toast } from "react-toastify";

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthStoreType {
  token: Request<string | null>;
  user: Request<User | null>;
  team: Request<Team | null>;
  login: (data: LoginDto) => Promise<void>;
  fetchUserData: () => Promise<void>;
  fetchUserTeam: () => Promise<void>;
  logout: () => void;
}

export const authStore = (
  set: SetAppState,
  get: GetAppState
): AuthStoreType => ({
  token: {
    data: null,
    isLoading: false,
    errors: []
  },
  user: {
    data: null,
    isLoading: false,
    errors: []
  },
  team: {
    data: null,
    isLoading: false,
    errors: []
  },
  login: async (data: LoginDto) => {
    get().auth.logout()
    if(get().auth.token.controller?.abort){
      get().auth.token.controller?.abort();
    }
    set(
      produce((draft: AppState) => {
        draft.auth.token.data = null;
        draft.auth.token.isLoading = true;
        draft.auth.token.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.post(`${apiUrl}/auth/login`, data, {
        signal: get().auth.token.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.auth.token.data = response.data.token_;
          return draft;
        })
      )
      toast.success("User logged successfully")
      set(
        produce((draft: AppState) => {
          draft.auth.token.isLoading = false;
          return draft;
        })
      )
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      set(
        produce((draft: AppState) => {
          draft.auth.token.isLoading = false;
          return draft;
        })
      )
      throw error
    }
  },
  fetchUserData: async () => {
    if(get().auth.user.controller?.abort){
      get().auth.user.controller?.abort();
    }
    set(
      produce((draft: AppState) => {
        draft.auth.user.isLoading = true;
        draft.auth.user.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/user/me`, {
        signal: get().auth.user.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.auth.user.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.auth.user.isLoading = false;
        return draft;
      })
    )
  },
  fetchUserTeam: async () => {
    if(get().auth.team.controller?.abort){
      get().auth.team.controller?.abort();
    }
    set(
      produce((draft: AppState) => {
        draft.auth.team.isLoading = true;
        draft.auth.team.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/team/me`, {
        signal: get().auth.team.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.auth.team.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      //notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.auth.team.isLoading = false;
        return draft;
      })
    )
  },
  logout: () => {
    set(
      produce((draft: AppState) => {
        draft.auth.token.data = null;
        draft.auth.user.data = null;
        draft.auth.team.data = null;
        return draft;
      })
    )
    get().pole.clearData();
    get().user.clearData();
    get().notification.clearData();
    get().team.clearData();
    get().repair.clearData();
  }
})