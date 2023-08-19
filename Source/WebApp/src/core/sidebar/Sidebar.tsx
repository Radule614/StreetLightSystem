import clsx from "clsx";
import logo from "../../logo.svg";
import { Navigation } from "../navigation";
import { isMobile } from "react-device-detect";
import { Button } from "../../shared";
import { ChevronLeftIcon } from "./Icon";

export const Sidebar = ({ closeHandler }: { closeHandler?: () => void }) => {
  return (
    <div
      className={clsx(
        "bg-light-2 top-0 left-0 p-2 flex flex-col z-[200]",
        isMobile
          ? "fixed w-full h-full min-h-screen"
          : "sticky h-screen border-r-[1px] border-light-3"
      )}
    >
      <div>
        <img src={logo} className="w-20 h-20" alt="logo" />
        <Button
          className="[&>div]:w-5 p-2 absolute top-2 right-2"
          variant="alternative"
          onClick={closeHandler}
        >
          <div>
            <ChevronLeftIcon />
          </div>
        </Button>
      </div>
      <div className="flex-1 overflow-auto">
        <Navigation />
      </div>
      <div className="flex p-2 items-center font-bold">
        <img
          className="w-10 h-10 rounded-full bg-light-3"
          src={logo}
          alt="user"
        />
        <div className="ml-4">
          <div className="text-dark-2 text-sm mb-1">Rade Stojanovic</div>
          <div className="text-dark-3 text-xs">@Radule614</div>
        </div>
      </div>
    </div>
  );
};
