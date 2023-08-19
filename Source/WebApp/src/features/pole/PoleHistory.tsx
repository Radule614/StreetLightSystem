import { twMerge } from "tailwind-merge";
import { AppState, appStore } from "../../core/store";
import {
  ButtonDropdown,
  DropdownAction,
  Loader,
  Repair,
  Team,
  convertDate,
  tableContentStyles,
  tableHeaderStyles,
} from "../../shared";
import { useEffect, useState } from "react";
import { EndRepairModal } from "../repair";

export const PoleHistory = ({ poleId }: { poleId: string }) => {
  const history = appStore((state: AppState) => state.repair.poleHistory.data);
  const fetchHistory = appStore(
    (state: AppState) => state.repair.fetchPoleHistory
  );
  const isLoading = appStore(
    (state: AppState) => state.repair.poleHistory.isLoading
  );
  const user = appStore((state: AppState) => state.auth.user.data);
  const [repairId, setRepairId] = useState<string | null>(null);
  const [isRepairVisible, setRepairVisible] = useState(false);

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
    fetchHistory(poleId);
  }, [fetchHistory, poleId]);

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

  const isOwnTeam = (team?: Team) => {
    return team?.members?.map((m) => m.id).includes(user?.id ?? "");
  };

  return (
    <div>
      <div
        className={twMerge(tableHeaderStyles, "grid-cols-9 z-10 top-[-31px]")}
      >
        <div className="col-span-2">Team Name</div>
        <div className="col-span-2">Start Date</div>
        <div className="col-span-2">End Date</div>
        <div className="col-span-2">Status</div>
        <div className="text-right">Action</div>
      </div>
      {isLoading && <Loader className="flex justify-center pt-24 relative" />}
      {!isLoading && history && history.length === 0 && (
        <div className="p-3 text-center">History empty</div>
      )}
      {!isLoading &&
        history &&
        history.length > 0 &&
        history?.map((repair) => (
          <div
            key={repair.id}
            className={twMerge(tableContentStyles, "grid-cols-9")}
          >
            <div className="text-xs col-span-2">
              {repair?.team && repair?.team?.name}
              {!repair?.team && <i>[deleted]</i>}
            </div>
            <div className="col-span-2 text-xs">
              {convertDate(repair.startDate).toLocaleString()}
            </div>
            <div className="col-span-2 text-xs">
              {repair.endDate !== "" &&
                convertDate(repair.endDate).toLocaleString()}
              {repair.endDate === "" && "-"}
            </div>
            <div className="col-span-2 text-xs">{statusToString(repair)}</div>
            <div className="flex justify-end gap-2 [&>*]:py-2 [&>*]:px-3">
              {!repair.isFinished && isOwnTeam(repair.team) && (
                <ButtonDropdown
                  fromModal={true}
                  actions={getActions(repair)}
                  label="Actions"
                />
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
