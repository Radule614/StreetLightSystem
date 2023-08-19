import { Navigate } from "react-router-dom";
import { AppState, appStore } from "../../core/store";

interface Props {
  children: any;
  roles: string[];
}

export const AuthorizationWrapper = ({ children, roles }: Props) => {
  const userData = appStore((state: AppState) => state.auth.user.data);
  if (roles.length === 0) return children;
  if (!userData || !userData.roles) return <Navigate to="/" replace />;

  for (let role of roles) {
    if (userData.roles?.find((r) => r.name === role)) return children;
  }
  return <Navigate to="/" replace />;
};
