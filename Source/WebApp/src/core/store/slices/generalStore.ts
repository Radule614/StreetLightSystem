import { AppState, GetAppState, SetAppState } from "../store";
import { produce } from "immer";

export interface GeneralStoreType {
  sidebarOpen: boolean;
  setSidebar: (state: boolean) => void;
}

export const generalStore = (
  set: SetAppState,
  get: GetAppState
): GeneralStoreType => ({
  sidebarOpen: false,
  setSidebar: (state: boolean) => {
    set(
      produce((draft: AppState) => {
        draft.general.sidebarOpen = state;
        return draft;
      })
    )
  }
});