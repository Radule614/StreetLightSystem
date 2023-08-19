import { Button } from "..";
import Popup from "reactjs-popup";
import { twMerge } from "tailwind-merge";

export interface DropdownAction {
  id: string;
  label: string;
  variant?: "default" | "danger";
  action: () => void;
}

const variantColor = {
  default: "text-black",
  danger: "text-red-600",
};

export const ButtonDropdown = ({
  actions,
  label,
  fromModal = false,
}: {
  label: string;
  actions: DropdownAction[];
  fromModal?: boolean;
}) => {
  const zIndex = fromModal ? 6000 : 4000;
  return (
    <Popup
      contentStyle={{ width: "150px", zIndex: zIndex }}
      trigger={
        <Button type="button" variant="alternative">
          <div className="flex items-center">
            {label}
            <svg
              className="w-2.5 h-2.5 ml-2.5"
              aria-hidden="true"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 10 6"
            >
              <path
                stroke="currentColor"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="m1 1 4 4 4-4"
              />
            </svg>
          </div>
        </Button>
      }
      position="bottom right"
    >
      <ul>
        {actions.map((action) => (
          <li key={action.id}>
            <button
              className={twMerge(
                "w-full py-2 border-light-3 border-b-[1px] text-sm",
                variantColor[action.variant ?? "default"],
                !action.variant || action.variant === "default"
                  ? "hover:bg-blue-50"
                  : "hover:bg-[#f6f6f6]"
              )}
              onClick={() => action.action()}
            >
              {action.label}
            </button>
          </li>
        ))}
      </ul>
    </Popup>
  );
};
