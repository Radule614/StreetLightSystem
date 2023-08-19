import { NavLink } from "react-router-dom";
import { NavRoute } from "./nav-route";
import clsx from "clsx";

export const SubNavigation = ({ routes }: { routes: NavRoute[] }) => {
  return (
    <nav className="flex border-light-3 border-b-[1px]">
      {routes.map((route) => (
        <NavLink
          className={({ isActive }) =>
            clsx(
              "flex items-center p-2 pr-4 border-light-3 border-r-[1px]",
              isActive
                ? "[&>div]:text-light-1 bg-blue-700"
                : "[&>div]:hover:text-blue-700 "
            )
          }
          key={route.link}
          to={route.link}
        >
          <div className="w-5 mr-3 text-dark-3">{route.icon}</div>
          <div className="text-sm font-bold text-dark-2">{route.value}</div>
        </NavLink>
      ))}
    </nav>
  );
};
