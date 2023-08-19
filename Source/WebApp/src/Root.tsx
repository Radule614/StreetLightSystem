import { AppState, appStore } from "./core/store";
import { Outlet } from "react-router-dom";
import { Header } from "./core/header";
import { Sidebar } from "./core/sidebar";
import { Footer } from "./core/footer";
import { isMobile } from "react-device-detect";
import { useEffect } from "react";

export const Root = () => {
  const isSidebarOpen = appStore(
    (state: AppState) => state.general.sidebarOpen
  );
  const changeSidebarState = appStore(
    (state: AppState) => state.general.setSidebar
  );

  useEffect(() => {
    if (!isMobile) {
      changeSidebarState(true);
    }
  }, [changeSidebarState]);

  return (
    <div className="relative flex">
      {isSidebarOpen && (
        <Sidebar closeHandler={() => changeSidebarState(false)} />
      )}
      <div className="w-full">
        <Header />
        <main className="min-h-[100vh]">
          <Outlet />
        </main>
        <Footer />
      </div>
    </div>
  );
};
