import { twMerge } from "tailwind-merge";
import {
  Dialog,
  Loader,
  Team,
  tableContentStyles,
  tableHeaderStyles,
} from "../../../shared";
import { AppState, appStore } from "../../../core/store";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ButtonDropdown, DropdownAction } from "../../../shared";

export const TeamList = () => {
  const teams = appStore((state: AppState) => state.team.teams.data);
  const fetchTeams = appStore((state: AppState) => state.team.fetchTeams);
  const isLoading = appStore((state: AppState) => state.team.teams.isLoading);
  const deleteTeam = appStore((state: AppState) => state.team.deleteTeam);
  const [team, setTeam] = useState<Team | null>(null);
  const [isDeleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const navigate = useNavigate();

  const handleDelete = (team: Team) => {
    setTeam(team);
    setDeleteDialogOpen(true);
  };

  const onDelete = async (teamId: string) => {
    try {
      await deleteTeam(teamId);
      fetchTeams();
    } catch (e) {}
  };

  useEffect(() => {
    fetchTeams();
  }, [fetchTeams]);

  const getActions = (team: Team): DropdownAction[] => [
    {
      id: "DeleteTeam",
      label: "Delete",
      variant: "danger",
      action: () => {
        handleDelete(team);
      },
    },
    {
      id: "TeamDetails",
      label: "Details",
      action: () => {
        navigate(`/team/details/${team.id}`);
      },
    },
  ];

  return (
    <div>
      <div className={twMerge(tableHeaderStyles, "grid-cols-7 z-10")}>
        <div className="col-span-2">Name</div>
        <div className="col-span-4">Members</div>
        <div className="text-right">Action</div>
      </div>
      {isLoading && <Loader className="flex justify-center pt-24 relative" />}
      {!isLoading &&
        teams?.map((team) => (
          <div
            key={team.id}
            className={twMerge(tableContentStyles, "grid-cols-7")}
          >
            <div className="col-span-2">{team.name}</div>
            <div className="col-span-4">
              {team.members?.map((member) => (
                <span key={member.id}>
                  {member.firstName}&nbsp;{member.lastName}
                  <br />
                </span>
              ))}
            </div>
            <div className="flex justify-end gap-2 [&>*]:py-2 [&>*]:px-3">
              <ButtonDropdown actions={getActions(team)} label="Actions" />
            </div>
          </div>
        ))}
      <Dialog
        title="Delete Team"
        content="Are you sure you want to delete this team?"
        action={() => onDelete(team?.id ?? "")}
        isVisible={isDeleteDialogOpen}
        onClose={(cancel: boolean) => {
          setTeam(null);
          setDeleteDialogOpen(false);
        }}
      />
    </div>
  );
};
