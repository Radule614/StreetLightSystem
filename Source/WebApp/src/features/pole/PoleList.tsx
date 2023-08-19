import { twMerge } from "tailwind-merge";
import {
  ButtonDropdown,
  DropdownAction,
  Pole,
  statusToColor,
  statusToString,
  tableContentStyles,
  tableHeaderStyles,
} from "../../shared";
import { AppState, appStore } from "../../core/store";

export const PoleList = ({
  poles,
  className,
  onRepairAction,
  onDetailsAction,
}: {
  poles: Pole[];
  className?: string;
  onRepairAction?: (pole: Pole) => void;
  onDetailsAction?: (pole: Pole) => void;
}) => {
  const team = appStore((state: AppState) => state.auth.team.data)
  const getActions = (pole: Pole): DropdownAction[] => [
    {
      id: "PoleDetails",
      label: "Details",
      action: () => {
        if (onDetailsAction) onDetailsAction(pole);
      },
    },
  ];

  const getBrokenPoleActions = (pole:Pole): DropdownAction[] => [
    {
      id: "PoleRepair",
      label: "Repair",
      action: () => {
        if (onRepairAction) onRepairAction(pole);
      },
    },
  ]

  return (
    <div className={className}>
      <div className={twMerge(tableHeaderStyles, "grid-cols-8 z-10")}>
        <div className="col-span-3">Pole ID</div>
        <div>Status</div>
        <div className="text-right">Latitude</div>
        <div className="text-right">Longitude</div>
        <div className="text-right col-span-2">Action</div>
      </div>
      {poles.map((pole) => (
        <div
          key={pole.id}
          className={twMerge(tableContentStyles, "grid-cols-8")}
        >
          <div className="col-span-3 text-xs">{pole.id}</div>
          <div className={twMerge(statusToColor(pole.status), "font-bold")}>
            {statusToString(pole.status)}
          </div>
          <div className="text-right">{pole.latitude}</div>
          <div className="text-right">{pole.longitude}</div>
          <div className="col-span-2 flex justify-end gap-2 [&>*]:py-2 [&>*]:px-3">
            {(pole.status !== 1 || !team) && (
              <ButtonDropdown actions={getActions(pole)} label="Actions" />
            )}
            {pole.status === 1 && team && (
              <ButtonDropdown actions={[...getActions(pole), ...getBrokenPoleActions(pole)]} label="Actions" />
            )}
          </div>
        </div>
      ))}
    </div>
  );
};
