import { Modal } from "flowbite-react";
import { Button, User } from "../../../shared";
import { twMerge } from "tailwind-merge";
import { Input } from "../../../shared/components/Input";
import { AppState, appStore } from "../../../core/store";
import { useEffect, useRef, useState } from "react";

const useFocus = () => {
  const htmlElRef = useRef(null);
  const setFocus = () => {
    htmlElRef.current && (htmlElRef.current as any).focus();
  };

  return [htmlElRef, setFocus];
};

export const MessageModal = ({
  user,
  onClose,
  isVisible,
  className,
}: {
  user: User | null;
  className?: string;
  onClose?: () => void;
  isVisible: boolean;
}) => {
  const sendMessage = appStore(
    (state: AppState) => state.user.sendMessageToUser
  );
  const [message, setMessage] = useState<string>("");
  const handleSubmit = async (e: any) => {
    e.preventDefault();
    try {
      await sendMessage({ message, userId: user?.id ?? "" });
      setMessage("");
      onClose && onClose();
    } catch (error) {}
  };
  const [inputRef, setInputFocus] = useFocus();
  useEffect(() => {
    (setInputFocus as () => void)();
  }, [message, setInputFocus]);
  return (
    <Modal
      show={isVisible}
      onClose={onClose}
      className={twMerge("[&>div]:w-[450px] z-[5000]", className)}
    >
      <form key="user-message-form" onSubmit={handleSubmit}>
        <Modal.Header>
          Send message to {user?.firstName}&nbsp;{user?.lastName}
        </Modal.Header>
        <Modal.Body>
          <Input
            ref={inputRef}
            id="user-message"
            key="user-message"
            label="message"
            value={message}
            onChange={(e: any) => setMessage(e.target.value)}
          />
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
            <Button type="submit">Send Message</Button>
          </>
        </Modal.Footer>
      </form>
    </Modal>
  );
};
