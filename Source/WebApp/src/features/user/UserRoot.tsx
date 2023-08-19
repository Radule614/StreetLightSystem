import { Outlet } from "react-router-dom";
import { NavRoute, SubNavigation } from "../../core/navigation";
import { CreateIcon, ListIcon } from "../../shared/Icon";

const Routes: NavRoute[] = [
  {
    link: "all",
    value: "Users",
    icon: <ListIcon />,
  },
  {
    link: "register",
    value: "Register User",
    icon: <CreateIcon />,
  },
];

export const UserRoot = () => {
  return (
    <div>
      <SubNavigation routes={Routes} />
      <section className="p-6 pb-32">
        <Outlet />
      </section>
    </div>
  );
};
