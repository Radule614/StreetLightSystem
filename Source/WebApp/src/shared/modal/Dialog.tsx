import { Modal } from "flowbite-react";
import { twMerge } from "tailwind-merge";
import { Button } from "../components/Button";

export const Dialog = ({
  title,
  content,
  action,
  isVisible,
  onClose,
  className,
}: {
  title: string;
  content: string;
  action: () => Promise<void>;
  isVisible: boolean;
  onClose: (cancel: boolean) => void;
  className?: string;
}) => {
  const handleSubmit = async (e: { preventDefault: () => void }) => {
    e.preventDefault();
    try {
      await action();
      onClose(false);
    } catch (err) {}
  };
  return (
    <Modal
      show={isVisible}
      onClose={() => onClose(true)}
      className={twMerge("[&>div]:w-[450px] z-[5000]", className)}
    >
      <form onSubmit={handleSubmit}>
        <Modal.Header>{title}</Modal.Header>
        <Modal.Body>{content}</Modal.Body>
        <Modal.Footer className="flex justify-end">
          <Button
            type="button"
            variant="alternative"
            onClick={() => onClose(true)}
          >
            Cancel
          </Button>
          <Button type="submit">Confirm</Button>
        </Modal.Footer>
      </form>
    </Modal>
  );
};
