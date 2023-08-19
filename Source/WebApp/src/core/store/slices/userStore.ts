import { produce } from "immer";
import { User, extractErrorMessages, notifyErrors } from "../../../shared";
import { AppState, GetAppState, Request, SetAppState, apiUrl } from "../store";
import axios from "axios";
import { toast } from "react-toastify";

export interface UserDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface UserStoreType {
  users: Request<User[]>;
  userData: Request<User>;
  fetchUsers: () => Promise<void>;
  fetchUserData: (userId: string) => Promise<void>
  createUser: (user: UserDto) => Promise<void>
  updateUser: (userId: string, user: UserDto) => Promise<void>;
  deleteUser: (userId: string) => Promise<void>
  sendMessageToUser: ({ userId, message }: { userId: string, message: string }) => Promise<void>,
  clearData: () => void;
}

export const userStore = (
  set: SetAppState,
  get: GetAppState
): UserStoreType => ({
  users: {
    data: null,
    isLoading: false,
    errors: []
  },
  userData: {
    data: null,
    isLoading: false,
    errors: []
  },
  fetchUsers: async () => {
    get().user.users.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.user.users.isLoading = true;
        draft.user.users.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/user`, {
        signal: get().user.users.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.user.users.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
    }
    set(
      produce((draft: AppState) => {
        draft.user.users.isLoading = false;
        return draft;
      })
    )
  },
  fetchUserData: async (userId: string) => {
    get().user.userData.controller?.abort();
    set(
      produce((draft: AppState) => {
        draft.user.userData.data = null;
        draft.user.userData.isLoading = true;
        draft.user.userData.controller = new AbortController();
        return draft;
      })
    )
    try {
      const response = await axios.get(`${apiUrl}/user/${userId}`, {
        signal: get().user.userData.controller?.signal
      })
      set(
        produce((draft: AppState) => {
          draft.user.userData.data = response.data;
          return draft;
        })
      )
    } catch (error: any) {
      if (axios.isCancel(error))
        return;
    }
    set(
      produce((draft: AppState) => {
        draft.user.userData.isLoading = false;
        return draft;
      })
    )
  },
  createUser: async (user: UserDto) => {
    try {
      await axios.post(`${apiUrl}/user`, user)
      toast.info("User data sent.")
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  updateUser: async (userId: string, user: UserDto) => {
    try {
      await axios.put(`${apiUrl}/user`, { ...user, id: userId })
      toast.info("User data sent.")
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  deleteUser: async (userId: string) => {
    try {
      await axios.delete(`${apiUrl}/user/${userId}`)
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  sendMessageToUser: async ({ userId, message }: { userId: string, message: string }) => {
    const data = {
      message: message,
      receiverId: userId,
      senderId: get().auth.user.data?.id
    }
    try {
      await axios.post(`${apiUrl}/notification/message`, data)
      toast.success("Message sent to user successfully")
    } catch (error: any) {
      notifyErrors(extractErrorMessages(error))
      throw error
    }
  },
  clearData: () => {
    set(
      produce((draft: AppState) => {
        draft.user.users.data = null;
        draft.user.users.isLoading = true;
        return draft;
      })
    )
  }
})