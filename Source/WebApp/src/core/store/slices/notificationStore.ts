import { produce } from "immer";
import { Message, extractErrorMessages, notifyErrors } from "../../../shared";
import { AppState, GetAppState, Request, SetAppState, apiUrl } from "../store";
import axios from "axios";

export interface NotificationStoreType {
  messages: Request<Message[]>;
  fetchUserMessages: () => Promise<void>;
  checkUnsentNotifications: () => Promise<void>;
  clearData: () => void;
}

export const notificationStore = (
  set: SetAppState,
  get: GetAppState
): NotificationStoreType => ({
  messages: {
    data: null,
    isLoading: false,
    errors: []
  },
  fetchUserMessages: async () => {
    get().notification.messages.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.notification.messages.isLoading = true;
        draft.notification.messages.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/notification/message`, {
        signal: get().notification.messages.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.notification.messages.data = response.data.messages;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
    }
    set(
      produce((draft: AppState) => {
        draft.notification.messages.isLoading = false;
        return draft;
      })
    )
  },
  checkUnsentNotifications: async () => {
    try {
      await axios.get(`${apiUrl}/notification/unsent`)
    } catch (error: any) {
      if(error?.response?.status !== 401)
        notifyErrors(extractErrorMessages(error))
    }
  },
  clearData: () => {
    set(
      produce((draft: AppState) => {
        draft.notification.messages.data = null;
        draft.notification.messages.isLoading = true;
        return draft;
      })
    )
  }
})