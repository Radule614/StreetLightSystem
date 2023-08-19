import { StoreApi, UseBoundStore, create } from "zustand";
import { mergeDeepRight } from "ramda"
import { immer } from "zustand/middleware/immer"
import { devtools, persist } from "zustand/middleware"
import { PoleStoreType, poleStore } from "./slices/poleStore";
import { GeneralStoreType, generalStore } from "./slices/generalStore";
import { UserStoreType, userStore } from "./slices/userStore";
import { AuthStoreType, authStore } from "./slices/authStore";
import { NotificationStoreType, notificationStore } from "./slices/notificationStore";
import { TeamStoreType, teamStore } from "./slices/teamStore";
import { deepCopy } from "../../shared/utility";
import { RepairStoreType, repairStore } from "./slices/repairStore";

export interface AppState {
  general: GeneralStoreType
  pole: PoleStoreType
  user: UserStoreType
  auth: AuthStoreType
  notification: NotificationStoreType
  team: TeamStoreType
  repair: RepairStoreType
}

export const apiUrl = process.env.REACT_APP_API_URL
export type SetAppState = StoreApi<AppState>["setState"]
export type GetAppState = StoreApi<AppState>["getState"]

const storeGenerator = (set: SetAppState, get: GetAppState): AppState => ({
  general: generalStore(set, get),
  pole: poleStore(set, get),
  user: userStore(set, get),
  auth: authStore(set, get),
  notification: notificationStore(set, get),
  team: teamStore(set, get),
  repair: repairStore(set, get)
})

const storeMerge = (persistedState: any, currentState: any) => {
  return mergeDeepRight(currentState, persistedState)
}

export const appStore: UseBoundStore<StoreApi<AppState>> = create(
  devtools(immer(persist(storeGenerator, {
    name: "app-store",
    merge: storeMerge,
    partialize: (state: AppState) => {
      const object: AppState = deepCopy(state);
      delete object.auth.token.controller;
      delete object.auth.user.controller;
      delete object.user.users.controller;
      delete object.user.userData.controller;
      delete object.team.teams.controller;
      delete object.team.members.controller;
      delete object.team.teamData.controller;
      delete object.pole.poles.controller;
      delete object.pole.poleDetails.controller;
      delete object.notification.messages.controller;
      delete object.repair.teamHistory.controller;
      delete object.repair.poleHistory.controller;
      return object;
    }
  })))
)
export interface Request<DataType> {
  data: DataType | null
  isLoading: boolean;
  errors: [];
  controller?: AbortController
}