import { twMerge } from "tailwind-merge";
import {
  Checkbox,
  Loader,
  tableContentStyles,
  tableHeaderStyles,
} from "../../shared";
import { AppState, appStore } from "../../core/store";
import { useEffect, useState } from "react";

export const MemberList = ({
  onCheckedMembers,
  initial,
}: {
  onCheckedMembers: (memberIds: string[]) => void;
  initial?: string[];
}) => {
  const members = appStore((state: AppState) => state.team.members.data);
  const fetchMembers = appStore((state: AppState) => state.team.fetchMembers);
  const isLoading = appStore((state: AppState) => state.team.members.isLoading);
  const [checkedMembers, setCheckedMembers] = useState<any>({});
  const [oldData, setOldData] = useState<string[]>([]);

  useEffect(() => {
    fetchMembers();
  }, [fetchMembers]);

  useEffect(() => {
    if (members && initial) {
      const checked: any = {};
      for (let member of members) {
        if (member.id) {
          checked[member.id] = initial?.includes(member.id) ? true : false;
        }
      }
      setCheckedMembers(checked);
      setOldData(initial);
    }
  }, [members, initial, oldData]);

  useEffect(() => {
    let memberIds = [];
    for (let id in checkedMembers) {
      if (checkedMembers[id] === true) memberIds.push(id);
    }
    onCheckedMembers(memberIds);
  }, [checkedMembers, onCheckedMembers]);

  return (
    <div>
      <div className={twMerge(tableHeaderStyles, "grid-cols-7 z-10")}>
        <div className="col-span-3">Member</div>
        <div className="col-span-3">Team</div>
        <div className="text-right">Selected</div>
      </div>
      {isLoading && <Loader className="flex justify-center pt-8 relative" />}
      {!isLoading &&
        members?.map((member) => (
          <div
            key={member.id}
            className={twMerge(
              tableContentStyles,
              "grid-cols-7 group cursor-pointer hover:bg-blue-50",
              checkedMembers[member.id] ? "bg-blue-50" : ""
            )}
            onClick={() => {
              if (member.id) {
                checkedMembers[member.id] = !checkedMembers[member.id];
                setCheckedMembers({ ...checkedMembers });
              }
            }}
          >
            <div
              className={twMerge(
                "col-span-3 name transition-all group-hover:pl-1.5",
                checkedMembers[member.id] ? "pl-1.5" : ""
              )}
            >
              {member.firstName}&nbsp;{member.lastName}
            </div>
            <div className="col-span-3">{member.team?.name ?? "-"}</div>
            <div className="flex justify-end">
              <Checkbox
                isChecked={checkedMembers[member.id ?? ""]}
                valueChanged={(val: boolean) => {
                  if (member.id) {
                    checkedMembers[member.id] = val;
                    setCheckedMembers({ ...checkedMembers });
                  }
                }}
              />
            </div>
          </div>
        ))}
    </div>
  );
};
