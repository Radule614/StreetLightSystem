import { useEffect } from "react";
import { AppState, appStore } from "../../core/store";
import { Button, Loader, SuccessActions } from "../../shared";
import { Modal } from "flowbite-react";
import { twMerge } from "tailwind-merge";
import { useNotification } from "../../core/notification";

export const StartRepairModal = ({
  poleId,
  onClose,
  isVisible,
  className,
}: {
  poleId: string;
  className?: string;
  onClose?: () => void;
  isVisible: boolean;
}) => {
  const fetchPoleById = appStore((state: AppState) => state.pole.fetchPoleById);
  const isLoading = appStore(
    (state: AppState) => state.pole.poleDetails.isLoading
  );
  const poleData = appStore((state: AppState) => state.pole.poleDetails.data);
  const startRepair = appStore(
    (state: AppState) => state.repair.startRepairProcess
  );
  const notification = useNotification();

  useEffect(() => {
    fetchPoleById(poleId);
  }, [fetchPoleById, poleId]);

  const handleSubmit = async (e: any) => {
    e.preventDefault();
    try {
      await startRepair(poleId);
    } catch (err) {}
  };

  useEffect(() => {
    if (notification?.action === SuccessActions.StartRepairSuccess && onClose) {
      onClose();
    }
  }, [notification, onClose]);

  return (
    <Modal
      show={isVisible}
      onClose={onClose}
      className={twMerge("[&>div]:w-[450px] z-[5500]", className)}
    >
      <form onSubmit={handleSubmit}>
        <Modal.Header>Start Repair Procedure</Modal.Header>
        <Modal.Body>
          {isLoading && <Loader className="flex justify-center py-5" />}
          {!isLoading && poleData && (
            <div>
              Clicking start button will start repair procedure and turn the pole into `being repaired` state.
            </div>
          )}
        </Modal.Body>
        <Modal.Footer className="flex justify-end">
          <>
            <Button
              type="button"
              variant="alternative"
              onClick={() => onClose && onClose()}
            >
              Cancel
            </Button>
            <Button type="submit">Start</Button>
          </>
        </Modal.Footer>
      </form>
    </Modal>
  );
};
