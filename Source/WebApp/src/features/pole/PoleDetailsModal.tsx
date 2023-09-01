import { useEffect } from "react";
import { AppState, appStore } from "../../core/store";
import {
  Button,
  Loader,
  Pole,
  SuccessActions,
  statusToColor,
  statusToString,
} from "../../shared";
import { twMerge } from "tailwind-merge";
import { Modal } from "flowbite-react";
import { PoleHistory } from "./PoleHistory";
import { notification$ } from "../../core/notification";
import { useSubscription } from "observable-hooks";

export const PoleDetailsModal = ({
  poleId,
  onClose,
  isVisible,
  onRepairAction,
  className,
}: {
  poleId: string;
  className?: string;
  onClose?: () => void;
  isVisible: boolean;
  onRepairAction?: (pole: Pole) => void;
}) => {
  const fetchPoleById = appStore((state: AppState) => state.pole.fetchPoleById);
  const isLoading = appStore(
    (state: AppState) => state.pole.poleDetails.isLoading
  );
  const poleData = appStore((state: AppState) => state.pole.poleDetails.data);
  const fetchHistory = appStore(
    (state: AppState) => state.repair.fetchPoleHistory
  );
  const team = appStore((state: AppState) => state.auth.team.data)

  useEffect(() => {
    fetchPoleById(poleId);
  }, [fetchPoleById, poleId]);

  useSubscription(notification$, (notification) => {
    if (
      notification?.action === SuccessActions.StartRepairSuccess ||
      notification?.action === SuccessActions.EndRepairSuccess
      ) {
        fetchPoleById(poleId);
        fetchHistory(poleId);
    }
  });

  return (
    <Modal
      show={isVisible}
      onClose={onClose}
      className={twMerge("[&>div]:w-[750px] z-[5000]", className)}
    >
      <Modal.Header>Pole Details</Modal.Header>
      <Modal.Body>
        {isLoading && <Loader className="flex justify-center py-5" />}
        {!isLoading && poleData && (
          <>
            <div className="flex flex-wrap pb-8 pt-1">
              <div className="w-2/5">Pole ID</div>
              <div className="w-3/5">{poleData.id}</div>
              <div className="w-2/5">Pole Status</div>
              <div className={twMerge("w-3/5", statusToColor(poleData.status))}>
                {statusToString(poleData.status)}
              </div>
              <div className="w-2/5">Latitude</div>
              <div className="w-3/5">{poleData.latitude}</div>
              <div className="w-2/5">Longitude</div>
              <div className="w-3/5">{poleData.longitude}</div>
            </div>
            <PoleHistory poleId={poleId} />
          </>
        )}
      </Modal.Body>
      <Modal.Footer className="flex justify-end">
        {poleData?.status === 1 && team && (
          <Button
            type="button"
            onClick={() => onRepairAction && onRepairAction(poleData)}
          >
            Repair
          </Button>
        )}
      </Modal.Footer>
    </Modal>
  );
};
