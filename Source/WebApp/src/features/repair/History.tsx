import { twMerge } from "tailwind-merge";
import { AppState, appStore } from "../../core/store";
import {
  ButtonDropdown,
  DropdownAction,
  Loader,
  Repair,
  SuccessActions,
  convertDate,
  tableContentStyles,
  tableHeaderStyles,
} from "../../shared";
import { useEffect, useState } from "react";
import { EndRepairModal } from "./EndRepairModal";
import { useNotification } from "../../core/notification";

export const History = () => {
  const history = appStore((state: AppState) => state.repair.teamHistory.data);
  const fetchHistory = appStore(
    (state: AppState) => state.repair.fetchTeamHistory
  );
  const isLoading = appStore(
    (state: AppState) => state.repair.teamHistory.isLoading
  );
  const userTeam = appStore((state: AppState) => state.auth.team.data);
  const [repairId, setRepairId] = useState<string | null>(null);
  const [isRepairVisible, setRepairVisible] = useState(false);
  const notification = useNotification();

  const getActions = (repair: Repair): DropdownAction[] => [
    {
      id: "FinishRepair",
      label: "Conclude",
      action: () => {
        setRepairId(repair.id);
        setRepairVisible(true);
      },
    },
  ];

  useEffect(() => {
    fetchHistory(userTeam?.id ?? "");
  }, [fetchHistory, userTeam?.id]);

  const statusToString = (repair: Repair) => {
    if (!repair.isFinished) {
      return <i>In Progress</i>;
    }
    return (
      <>
        {repair.isSuccessful && <i className="text-green-500">Success</i>}
        {!repair.isSuccessful && <i className="text-red-500">Failure</i>}
      </>
    );
  };

  useEffect(() => {
    if (notification?.action === SuccessActions.EndRepairSuccess) {
      fetchHistory(userTeam?.id ?? "");
    }
  }, [fetchHistory, notification, userTeam?.id]);

  return (
    <div>
      <div className={twMerge(tableHeaderStyles, "grid-cols-11 z-10")}>
        <div className="col-span-4">Pole Id</div>
        <div className="col-span-2">Start Date</div>
        <div className="col-span-2">End Date</div>
        <div className="col-span-2">Status</div>
        <div className="text-right">Action</div>
      </div>
      {isLoading && <Loader className="flex justify-center pt-24 relative" />}
      {!isLoading &&
        history?.map((repair) => (
          <div
            key={repair.id}
            className={twMerge(tableContentStyles, "grid-cols-11")}
          >
            <div className="text-xs col-span-4">{repair.poleId}</div>
            <div className="col-span-2">
              {convertDate(repair.startDate).toLocaleString()}
            </div>
            <div className="col-span-2">
              {repair.endDate !== "" &&
                convertDate(repair.endDate).toLocaleString()}
              {repair.endDate === "" && "-"}
            </div>
            <div className="col-span-2">{statusToString(repair)}</div>
            <div className="flex justify-end gap-2 [&>*]:py-2 [&>*]:px-3">
              {!repair.isFinished && (
                <ButtonDropdown actions={getActions(repair)} label="Actions" />
              )}
            </div>
          </div>
        ))}
      <EndRepairModal
        repairId={repairId ?? ""}
        isVisible={isRepairVisible}
        onClose={() => {
          setRepairVisible(false);
          setRepairId(null);
        }}
      />
    </div>
  );
};
