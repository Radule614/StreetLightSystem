import { Navigate, RouteObject } from "react-router-dom";
import { TeamList } from "./list";
import { CreateTeam } from "./create";
import { TeamDetails } from "./details";

export const teamRoutes: RouteObject[] = [
  {
    path: "all",
    element: <TeamList />,
  },
  {
    path: "create",
    element: <CreateTeam />,
  },
  {
    path: "details/:id",
    element: <TeamDetails />,
  },
  {
    path: "",
    element: <Navigate to="all" replace/>
  }
];
