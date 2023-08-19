import { useState } from "react";
import { Input } from "../../../shared/components/Input";
import { Button, deepEqual } from "../../../shared";
import { AppState, appStore } from "../../../core/store";
import { useNavigate } from "react-router-dom";
import { TeamDto } from "../../../core/store/slices/teamStore";
import { MemberList } from "../MemberList";

const getInitialData = (): TeamDto => {
  return {
    name: "",
    memberIds: [],
  };
};

export const CreateTeam = () => {
  const [data, setData] = useState<TeamDto>(getInitialData());
  const createTeam = appStore((state: AppState) => state.team.createTeam);
  const navigate = useNavigate();

  const handleSubmit = async (e: { preventDefault: () => void }) => {
    e.preventDefault();
    try {
      await createTeam(data);
      navigate("/team");
    } catch (err) {}
  };

  const handleReset = () => {
    setData(getInitialData());
  };

  const setMemberIds = (memberIds: string[]) => {
    if (!deepEqual(memberIds, data.memberIds)) {
      setData({ ...data, memberIds });
    }
  };

  return (
    <div className="max-w-[1000px] mx-auto mt-6">
      <form onSubmit={handleSubmit}>
        <div className="custom-grid">
          <Input
            label="Name"
            id="create-name"
            value={data.name}
            required
            onChange={(e: any) => setData({ ...data, name: e.target.value })}
          />
        </div>
        <div className="mt-8">
          <MemberList onCheckedMembers={setMemberIds} />
        </div>
        <div className="flex gap-3 mt-12 justify-end">
          <Button type="button" variant="alternative" onClick={handleReset}>
            Reset
          </Button>
          <Button type="submit">Create</Button>
        </div>
      </form>
    </div>
  );
};
