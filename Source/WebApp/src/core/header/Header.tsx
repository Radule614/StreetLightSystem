import { Button } from "../../shared";
import { getActiveRole } from "../../shared/utility";
import { ChevronRightIcon } from "../sidebar/Icon";
import { AppState, appStore } from "../store";

export const Header = () => {
  const isSidebarOpen = appStore(
    (state: AppState) => state.general.sidebarOpen
  );
  const changeSidebarState = appStore(
    (state: AppState) => state.general.setSidebar
  );
  const logout = appStore((state: AppState) => state.auth.logout);
  const userData = appStore((state: AppState) => state.auth.user.data);
  const userTeam = appStore((state: AppState) => state.auth.team.data);

  return (
    <header className="flex px-6 py-2 bg-light-2 border-b-[1px] border-light-3">
      <div className="flex-1 flex items-center">
        {!isSidebarOpen && (
          <Button
            className="[&>div]:w-5 p-2"
            variant="alternative"
            onClick={() => changeSidebarState(true)}
          >
            <div>
              <ChevronRightIcon />
            </div>
          </Button>
        )}
      </div>
      <div className="flex gap-5 items-center">
        {userData != null && (
          <div>
            <span>
              {userData?.firstName}&nbsp;{userData?.lastName}
            </span>
            {userTeam && (
              <span className="pl-1 italic text-blue-600 text-sm">
                ({userTeam.name})
              </span>
            )}
            {getActiveRole(userData)?.name === "Admin" && (
              <span className="pl-1 italic text-blue-600 text-sm">(Admin)</span>
            )}
          </div>
        )}
        <Button onClick={logout}>Logout</Button>
      </div>
    </header>
  );
};
