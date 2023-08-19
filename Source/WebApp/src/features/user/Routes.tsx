import { Navigate, RouteObject } from "react-router-dom";
import { UserList } from "./list";
import { CreateUser } from "./create";
import { UpdateUser } from "./update";

export const userRoutes: RouteObject[] = [
  {
    path: "all",
    element: <UserList />,
  },
  {
    path: "register",
    element: <CreateUser />,
  },
  {
    path: "update/:id",
    element: <UpdateUser />,
  },
  {
    path: "",
    element: <Navigate to="all" replace/>
  }
];
