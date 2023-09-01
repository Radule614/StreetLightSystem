import axios from "axios";
import { Pole, extractErrorMessages, notifyErrors } from "../../../shared";
import { AppState, GetAppState, SetAppState, apiUrl, Request } from "../store";
import { produce } from "immer";
import { noCachedDataWarn } from "../../../shared";

export interface PoleStoreType {
  poles: Request<Pole[]>;
  poleDetails: Request<Pole>;
  fetchPoles: () => Promise<void>
  fetchPoleById: (id: string) => Promise<void>
  clearData: () => void;
}

export const poleStore = (
  set: SetAppState,
  get: GetAppState
): PoleStoreType => ({
  poles: {
    data: null,
    isLoading: false,
    errors: []
  },
  poleDetails: {
    data: null,
    isLoading: false,
    errors: []
  },
  fetchPoles: async () => {
    get().pole.poles.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.pole.poles.data = null;
        draft.pole.poles.isLoading = true;
        draft.pole.poles.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/pole`, {
        signal: get().pole.poles.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.pole.poles.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      if (error.code === "ERR_NETWORK")
        noCachedDataWarn("Poles");
      else
        notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.pole.poles.isLoading = false;
        return draft;
      })
    )
  },
  fetchPoleById: async (id: string) => {
    if (id === "") return;
    get().pole.poleDetails.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.pole.poleDetails.data = null;
        draft.pole.poleDetails.isLoading = true;
        draft.pole.poleDetails.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/pole/${id}`, {
        signal: get().pole.poleDetails.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.pole.poleDetails.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
      if (error.code === "ERR_NETWORK")
        noCachedDataWarn("Pole data");
      else
        notifyErrors(extractErrorMessages(error))
    }
    set(
      produce((draft: AppState) => {
        draft.pole.poleDetails.isLoading = false;
        return draft;
      })
    )
  },
  clearData: () => {
    set(
      produce((draft: AppState) => {
        draft.pole.poles.data = null;
        draft.pole.poles.isLoading = true;
        draft.pole.poleDetails.data = null;
        draft.pole.poleDetails.isLoading = true;
        return draft;
      })
    )
  }
});