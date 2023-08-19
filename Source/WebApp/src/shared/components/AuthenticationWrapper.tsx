import { Navigate } from "react-router-dom";
import { AppState, appStore } from "../../core/store";

interface Props {
  children: any;
  inverse?: boolean;
}

export const AuthenticationWrapper = ({ children, inverse }: Props) => {
  const isLogged = appStore((state: AppState) => state.auth.token.data) !== null;
  if (inverse) return isLogged ? <Navigate to="/" replace /> : children;
  return isLogged ? children : <Navigate to="/login" replace />;
};