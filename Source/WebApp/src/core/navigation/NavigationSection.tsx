import { NavLink } from "react-router-dom";
import { NavRoute } from "./nav-route";
import clsx from "clsx";

export const NavigationSection = ({
  routes,
  title,
}: {
  routes: NavRoute[];
  title: string;
}) => {
  return (
    <>
      <h3 className="text-xs text-dark-3 mb-2">{title}</h3>
      <nav className="mb-5">
        {routes.map((route) => (
          <NavLink
            className={({ isActive }) =>
              clsx(
                "flex items-center p-2  pr-32",
                isActive
                  ? "[&>div]:text-light-1 bg-blue-700 rounded-lg"
                  : "[&>div]:hover:text-blue-700"
              )
            }
            key={route.link}
            to={route.link}
          >
            <div className="w-5 mr-4 text-dark-3">{route.icon}</div>
            <div className="text-sm font-bold text-dark-2">{route.value}</div>
          </NavLink>
        ))}
      </nav>
    </>
  );
};
