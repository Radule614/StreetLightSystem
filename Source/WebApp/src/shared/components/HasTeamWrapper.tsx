import { Navigate } from "react-router-dom";
import { AppState, appStore } from "../../core/store";

interface Props {
  children: any;
}

export const HasTeamWrapper = ({ children }: Props) => {
  const team = appStore((state: AppState) => state.auth.team.data);
  return team ? children : <Navigate to="/" replace />;
};