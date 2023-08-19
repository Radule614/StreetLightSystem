import { User } from "./user";

export interface Notification {
  message: string;
  action?: string;
  receiverId: string;
}

export interface Message {
  id: string;
  message: string;
  receiverId: string;
  sentDate: string;
  receivedDate: string;
  sender: User;
  isNew: boolean;
}

export const SuccessActions = {
  CreateUserSuccess: "CreateUserSuccess",
  DeleteUserSuccess: "DeleteUserSuccess",
  UpdateUserSuccess: "UpdateUserSuccess",
  StartRepairSuccess: "StartRepairSuccess",
  EndRepairSuccess: "EndRepairSuccess"
}

export const ErrorActions = {
  CreateUserFailure: "CreateUserFailure",
  DeleteUserFailure: "DeleteUserFailure",
  UpdateUserFailure: "UpdateUserFailure",
  StartRepairFailure: "StartRepairFailure",
  EndRepairFailure: "EndRepairFailure"
}

export const WarningActions = {
  PoleStatusChanged: "PoleStatusChanged"
}

export const isSuccessAction = (action: string) => {
  for (const key in SuccessActions) {
    if (action === key) return true;
  }
  return false;
}

export const isErrorAction = (action: string) => {
  for (const key in ErrorActions) {
    if (action === key) return true;
  }
  return false;
}

export const isWarningAction = (action: string) => {
  for (const key in WarningActions) {
    if (action === key) return true;
  }
  return false;
}