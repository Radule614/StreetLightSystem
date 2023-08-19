import { useEffect, useState } from "react";
import { Input } from "../../../shared/components/Input";
import { Button, SuccessActions } from "../../../shared";
import { AppState, appStore } from "../../../core/store";
import { UserDto } from "../../../core/store/slices/userStore";
import { toast } from "react-toastify";
import { useNotification } from "../../../core/notification";
import { useNavigate } from "react-router-dom";

const getInitialData = (): UserDto => {
  return {
    firstName: "",
    lastName: "",
    email: "",
    password: "",
  };
};

export const CreateUser = () => {
  const [data, setData] = useState<UserDto>(getInitialData());
  const [confirmPassword, setConfirmPassword] = useState<string>("");
  const createUser = appStore((state: AppState) => state.user.createUser);
  const notification = useNotification();
  const navigate = useNavigate();

  const handleSubmit = async (e: { preventDefault: () => void }) => {
    e.preventDefault();
    if (confirmPassword !== data.password) {
      toast.error("Passwords do not match.");
      return;
    }
    try {
      await createUser(data);
    } catch (err) {}
  };

  useEffect(() => {
    if (notification?.action === SuccessActions.CreateUserSuccess) {
      navigate("/user");
    }
  }, [navigate, notification]);

  const handleReset = () => {
    setData(getInitialData());
    setConfirmPassword("");
  };

  return (
    <div className="max-w-[1000px] mx-auto mt-6">
      <form onSubmit={handleSubmit}>
        <div className="custom-grid">
          <Input
            label="First Name"
            id="create-first-name"
            value={data.firstName}
            required
            onChange={(e: any) =>
              setData({ ...data, firstName: e.target.value })
            }
          />
          <Input
            label="Last Name"
            id="create-last-name"
            value={data.lastName}
            required
            onChange={(e: any) =>
              setData({ ...data, lastName: e.target.value })
            }
          />
          <Input
            label="Email"
            type="email"
            id="create-email"
            value={data.email}
            required
            onChange={(e: any) => setData({ ...data, email: e.target.value })}
          />
          <div></div>
          <Input
            label="Password"
            type="password"
            id="create-password"
            value={data.password}
            required
            onChange={(e: any) =>
              setData({ ...data, password: e.target.value })
            }
          />
          <Input
            label="Confirm Password"
            type="password"
            id="create-confirm-password"
            value={confirmPassword}
            required
            onChange={(e: any) => setConfirmPassword(e.target.value)}
          />
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
