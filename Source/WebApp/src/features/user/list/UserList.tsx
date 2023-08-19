import { twMerge } from "tailwind-merge";
import {
  ButtonDropdown,
  Dialog,
  DropdownAction,
  Loader,
  SuccessActions,
  User,
  tableContentStyles,
  tableHeaderStyles,
} from "../../../shared";
import { AppState, appStore } from "../../../core/store";
import { useEffect, useState } from "react";
import { MessageModal } from "./MessageModal";
import { useNotification } from "../../../core/notification";
import { useNavigate } from "react-router-dom";

export const UserList = () => {
  const users = appStore((state: AppState) => state.user.users.data);
  const fetchUsers = appStore((state: AppState) => state.user.fetchUsers);
  const isLoading = appStore((state: AppState) => state.user.users.isLoading);
  const deleteUser = appStore((state: AppState) => state.user.deleteUser);
  const [user, setUser] = useState<User | null>(null);
  const [isDeleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [isMessageDialogOpen, setMessageDialogOpen] = useState(false);
  const notification = useNotification();
  const navigate = useNavigate();

  const handleDelete = (user: User) => {
    setUser(user);
    setDeleteDialogOpen(true);
  };

  const sendMessage = (user: User) => {
    setUser(user);
    setMessageDialogOpen(true);
  };

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  useEffect(() => {
    if (notification?.action === SuccessActions.DeleteUserSuccess) {
      fetchUsers();
    }
  }, [fetchUsers, notification]);

  const getActions = (user: User): DropdownAction[] => [
    {
      id: "DeleteUser",
      label: "Delete",
      variant: "danger",
      action: () => {
        handleDelete(user);
      },
    },
    {
      id: "MessageUser",
      label: "Message",
      action: () => {
        sendMessage(user)
      }
    },
    {
      id: "UpdateUser",
      label: "Update",
      action: () => {
        navigate(`/user/update/${user.id}`);
      },
    }
  ];

  return (
    <div>
      <div className={twMerge(tableHeaderStyles, "grid-cols-5 z-10")}>
        <div>First Name</div>
        <div>Last Name</div>
        <div className="col-span-2">Email</div>
        <div className="text-right">Action</div>
      </div>
      {isLoading && <Loader className="flex justify-center pt-24 relative" />}
      {!isLoading &&
        users?.map((user) => (
          <div
            key={user.id}
            className={twMerge(tableContentStyles, "grid-cols-5")}
          >
            <div>{user.firstName}</div>
            <div>{user.lastName}</div>
            <div className="col-span-2">{user.email}</div>
            <div className="flex justify-end gap-2 [&>*]:py-2 [&>*]:px-3">
            <ButtonDropdown actions={getActions(user)} label="Actions" />
            </div>
          </div>
        ))}
      <Dialog
        title="Delete User"
        content="Are you sure you want to delete this user?"
        action={() => deleteUser(user?.id ?? "")}
        isVisible={isDeleteDialogOpen}
        onClose={(cancel: boolean) => {
          setUser(null);
          setDeleteDialogOpen(false);
        }}
      />
      <MessageModal
        user={user!}
        isVisible={isMessageDialogOpen && user != null}
        onClose={() => {
          setUser(null);
          setMessageDialogOpen(false);
        }}
      />
    </div>
  );
};
