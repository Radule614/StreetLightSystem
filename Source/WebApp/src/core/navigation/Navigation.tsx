import { NavRoute } from "./nav-route";
import {
  HomeIcon,
  PoleIcon,
  RepairIcon,
  TeamIcon,
  UsersIcon,
  SettingsIcon,
  InboxIcon,
} from "./Icon";
import { NavigationSection } from "./NavigationSection";
import { AppState, appStore } from "../store";
import { getActiveRole } from "../../shared/utility";

const UserRoutes: NavRoute[] = [
  {
    value: "Home",
    link: "/",
    icon: <HomeIcon />,
  },
  {
    value: "Poles",
    link: "/pole",
    icon: <PoleIcon />,
  },
  {
    value: "Settings",
    link: "/settings",
    icon: <SettingsIcon />,
  },
  {
    value: "Inbox",
    link: "/inbox",
    icon: <InboxIcon />,
  },
];

const TeamRoutes: NavRoute[] = [
  {
    value: "Repairs",
    link: "/repair",
    icon: <RepairIcon />,
  },
]

const AdminRoutes: NavRoute[] = [
  {
    value: "Users",
    link: "/user",
    icon: <UsersIcon />,
  },
  {
    value: "Teams",
    link: "/team",
    icon: <TeamIcon />,
  },
];

export const Navigation = () => {
  const userData = appStore((state: AppState) => state.auth.user.data);
  const team = appStore((state: AppState) => state.auth.team.data);
  return (
    <div className="px-4">
      <NavigationSection routes={UserRoutes} title="Navigation" />
      {team && (
        <NavigationSection routes={TeamRoutes} title="Team" />
      )}
      {getActiveRole(userData)?.name === "Admin" && (
        <NavigationSection routes={AdminRoutes} title="Admin" />
      )}
    </div>
  );
};
