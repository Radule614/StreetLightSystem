import { Outlet } from "react-router-dom";
import { NavRoute, SubNavigation } from "../../core/navigation";
import { CreateIcon, ListIcon } from "../../shared/Icon";

const Routes: NavRoute[] = [
  {
    link: "all",
    value: "Teams",
    icon: <ListIcon />,
  },
  {
    link: "create",
    value: "Create Team",
    icon: <CreateIcon />,
  },
];

export const TeamRoot = () => {
  return (
    <div>
      <SubNavigation routes={Routes} />
      <section className="p-6 pb-32">
        <Outlet />
      </section>
    </div>
  );
};
