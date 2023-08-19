import { createBrowserRouter } from "react-router-dom";
import { App } from "../../App";
import { ErrorPage } from "./ErrorPage";
import { PoleRoot } from "../../features/pole";
import { Login } from "../../features/auth";
import {
  AuthorizationWrapper,
  AuthenticationWrapper,
  HasTeamWrapper,
} from "../../shared";
import { UserRoot, userRoutes } from "../../features/user";
import { Root } from "../../Root";
import { InboxRoot } from "../../features/inbox";
import { HomeRoot } from "../../features/home";
import { TeamRoot, teamRoutes } from "../../features/team";
import { RepairRoot } from "../../features/repair";

export const router = createBrowserRouter([
  {
    path: "sw.js",
    element: null,
  },
  {
    path: "/",
    element: <App />,
    children: [
      {
        path: "/",
        element: (
          <AuthenticationWrapper>
            <Root />
          </AuthenticationWrapper>
        ),
        children: [
          {
            path: "",
            element: <HomeRoot />,
          },
          {
            path: "pole",
            element: <PoleRoot />,
          },
          {
            path: "user",
            element: (
              <AuthorizationWrapper roles={["Admin"]}>
                <UserRoot />
              </AuthorizationWrapper>
            ),
            children: userRoutes,
          },
          {
            path: "team",
            element: (
              <AuthorizationWrapper roles={["Admin"]}>
                <TeamRoot />
              </AuthorizationWrapper>
            ),
            children: teamRoutes,
          },
          {
            path: "repair",
            element: (
              <HasTeamWrapper>
                <RepairRoot />
              </HasTeamWrapper>
            ),
          },
          {
            path: "inbox",
            element: <InboxRoot />,
          },
          {
            path: "*",
            element: <ErrorPage />,
          },
        ],
      },
      {
        path: "/login",
        element: (
          <AuthenticationWrapper inverse={true}>
            <Login />
          </AuthenticationWrapper>
        ),
      },
    ],
  },
]);
