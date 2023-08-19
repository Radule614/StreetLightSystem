import { useEffect } from "react";
import { AppState, appStore } from "../../core/store";
import { Button, SuccessActions } from "../../shared";
import { Modal } from "flowbite-react";
import { twMerge } from "tailwind-merge";
import { useNotification } from "../../core/notification";

export const EndRepairModal = ({
  repairId,
  onClose,
  isVisible,
  className,
}: {
  repairId: string;
  className?: string;
  onClose?: () => void;
  isVisible: boolean;
}) => {
  const endRepair = appStore(
    (state: AppState) => state.repair.endRepairProcess
  );
  const notification = useNotification();

  const onSuccess = async (e: any) => {
    e.preventDefault();
    try {
      await endRepair(repairId, true);
    } catch (err) {}
  };

  const onFailure = async (e: any) => {
    try {
      await endRepair(repairId, false);
    } catch (err) {}
  };

  useEffect(() => {
    if (notification?.action === SuccessActions.EndRepairSuccess && onClose) {
      onClose();
    }
  }, [notification, onClose]);

  return (
    <Modal
      show={isVisible}
      onClose={onClose}
      className={twMerge("[&>div]:w-[450px] z-[7000]", className)}
    >
      <form onSubmit={onSuccess}>
        <Modal.Header>End Repair Procedure</Modal.Header>
        <Modal.Body>
          <div>
            Choose whether to end repair procedure with failure or success.
          </div>
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
            <Button type="button" variant="error" onClick={onFailure}>
              Failure
            </Button>
            <Button type="submit">Success</Button>
          </>
        </Modal.Footer>
      </form>
    </Modal>
  );
};
