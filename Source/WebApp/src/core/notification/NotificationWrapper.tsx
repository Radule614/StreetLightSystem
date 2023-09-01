import * as signalR from "@microsoft/signalr";
import { useEffect } from "react";
import { toast } from "react-toastify";
import { appStore, AppState } from "../store";
import {
  Message,
  Notification,
  isErrorAction,
  isSuccessAction,
  isWarningAction,
} from "../../shared";
import { Subject } from "rxjs";

export const notificationSubject$ = new Subject<Notification | null>();
export const notification$ = notificationSubject$.asObservable();


export const NotificationWrapper = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const userId = appStore((state: AppState) => state.auth.user.data?.id);
  const checkUnsentNotifications = appStore(
    (state: AppState) => state.notification.checkUnsentNotifications
  );

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl(`${process.env.REACT_APP_API_URL}/notification?userId=${userId}`)
      .build();
    connection.on("broadcast", (_: string, notification: Notification) => {
      notificationSubject$.next(notification);
      if (isWarningAction(notification.action ?? "")) {
        toast.warn(notification.message, { autoClose: 15000 });
      } else {
        toast.info(notification.message, { autoClose: 15000 });
      }
    });
    connection.on("notification", (_: string, notification: Notification) => {
      notificationSubject$.next(notification);
      if (isSuccessAction(notification.action ?? "")) {
        toast.success(notification.message, { autoClose: 15000 });
      } else if (isErrorAction(notification.action ?? "")) {
        toast.error(notification.message, { autoClose: 15000 });
      } else {
        toast.info(notification.message, { autoClose: 15000 });
      }
    });
    connection.on("message", (_: string, message: Message) => {
      toast.info(
        <>
          <div className="font-bold text-sm">
            Received message from&nbsp;
            {message.sender?.firstName}&nbsp;{message.sender?.lastName}:
          </div>
          <div>{message.message}</div>
        </>,
        { autoClose: 15000 }
      );
    });
    connection
      .start()
      .then(() => {
        checkUnsentNotifications();
      })
      .catch((e) => {});
    return () => {
      connection.stop();
    };
  }, [userId, checkUnsentNotifications]);

  return <>{children}</>;
};
