import { AppState, appStore } from "./core/store";
import { Outlet } from "react-router-dom";
import { useEffect } from "react";
import axios from "axios";
import { NotificationWrapper } from "./core/notification";

let tokenValue: string | null = null;

axios.interceptors.request.use(
  (config) => {
    if (tokenValue !== null) {
      config.headers.Authorization = `Bearer ${tokenValue}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export const App = () => {
  const token = appStore((state: AppState) => state.auth.token);
  const fetchUserData = appStore((state: AppState) => state.auth.fetchUserData);
  const fetchUserTeam = appStore((state: AppState) => state.auth.fetchUserTeam);

  useEffect(() => {
    tokenValue = token.data;
    if (token.data != null) {
      fetchUserData();
      fetchUserTeam();
    }
  }, [token, fetchUserData, fetchUserTeam]);

  return (
    <NotificationWrapper>
      <Outlet />
    </NotificationWrapper>
  )
};
