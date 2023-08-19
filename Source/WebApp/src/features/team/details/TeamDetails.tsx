import { useEffect, useState } from "react";
import { Input } from "../../../shared/components/Input";
import { Button, Loader, deepEqual } from "../../../shared";
import { AppState, appStore } from "../../../core/store";
import { useNavigate, useParams } from "react-router-dom";
import { TeamDto } from "../../../core/store/slices/teamStore";
import { MemberList } from "../MemberList";

const getInitialData = (): TeamDto => {
  return {
    name: "",
    memberIds: [],
  };
};

export const TeamDetails = () => {
  const [data, setData] = useState<TeamDto>(getInitialData());
  const teamData = appStore((state: AppState) => state.team.teamData);
  const fetchTeamData = appStore((state: AppState) => state.team.fetchTeamData);
  const updateTeam = appStore((state: AppState) => state.team.updateTeam);
  const [initialMembers, setInitialMembers] = useState<string[]>([]);
  const params = useParams();
  const navigate = useNavigate();

  const handleSubmit = async (e: { preventDefault: () => void }) => {
    e.preventDefault();
    try {
      await updateTeam(params.id ?? "", data);
      navigate("/team");
    } catch (err) {}
  };

  useEffect(() => {
    fetchTeamData(params.id ?? "");
  }, [fetchTeamData, params.id]);

  useEffect(() => {
    setData({
      name: teamData.data?.name ?? "",
      memberIds: [],
    });
    setInitialMembers(teamData.data?.members?.map((member) => member.id) ?? []);
  }, [teamData]);

  const handleReset = () => {
    const data = {
      ...getInitialData(),
      name: teamData.data?.name ?? "",
    };
    setData(data);
    setInitialMembers(teamData.data?.members?.map((member) => member.id) ?? []);
  };

  const setMemberIds = (memberIds: string[]) => {
    if (!deepEqual(memberIds, data.memberIds)) {
      setData({ ...data, memberIds });
    }
  };

  return (
    <div className="max-w-[1000px] mx-auto mt-6">
      <h2 className="pb-5 text-xl">Team Details</h2>
      {teamData.isLoading && (
        <Loader size="lg" className="flex justify-center pt-10" />
      )}
      {!teamData.isLoading && (
        <form onSubmit={handleSubmit}>
          <div className="custom-grid">
            <Input
              label="First Name"
              id="update-name"
              value={data.name}
              required
              onChange={(e: any) => setData({ ...data, name: e.target.value })}
            />
          </div>
          <div className="mt-8">
            <MemberList
              onCheckedMembers={setMemberIds}
              initial={initialMembers}
            />
          </div>
          <div className="flex gap-3 mt-12 justify-end">
            <Button type="button" variant="alternative" onClick={handleReset}>
              Reset
            </Button>
            <Button type="submit">Update</Button>
          </div>
        </form>
      )}
    </div>
  );
};
