import { useEffect } from "react";
import { AppState, appStore } from "../../core/store";
import { Loader, convertDate, tableContentStyles, tableHeaderStyles } from "../../shared";
import { twMerge } from "tailwind-merge";

export const InboxRoot = () => {
  const messageData = appStore(
    (state: AppState) => state.notification.messages.data
  );
  const isLoading = appStore(
    (state: AppState) => state.notification.messages.isLoading
  );
  const fetchUserMessages = appStore(
    (state: AppState) => state.notification.fetchUserMessages
  );

  useEffect(() => {
    fetchUserMessages();
  }, [fetchUserMessages]);

  return (
    <section className="p-6 pb-32">
      <h2 className="pb-3 text-xl">Inbox</h2>
      <div>
        <div className={twMerge(tableHeaderStyles, "grid-cols-12 z-10")}>
          <div>No</div>
          <div className="col-span-2">Sender</div>
          <div className="col-span-5">Content</div>
          <div className="col-span-2">Date Sent</div>
          <div className="col-span-2">Date Received</div>
        </div>
        {isLoading && <Loader className="flex justify-center pt-24 relative" />}
        {!isLoading &&
          messageData?.map((message, index) => (
            <div
              key={message.id}
              className={twMerge(tableContentStyles, "grid-cols-12")}
            >
              <div className="max-w-5">{index + 1}</div>
              <div className="flex-wrap flex items-center col-span-2">
                <div>{message.sender.firstName}&nbsp;</div>
                <div className="pr-1">{message.sender.lastName}</div>
                {message.isNew && (
                  <div className="italic text-blue-600 text-xs">(new)</div>
                )}
              </div>
              <div className="col-span-5">{message.message}</div>
              <div className="col-span-2">{convertDate(message.sentDate).toLocaleString()}</div>
              <div className="col-span-2">{convertDate(message.receivedDate).toLocaleString()}</div>
            </div>
          ))}
      </div>
    </section>
  );
};
