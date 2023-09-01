import { useEffect, useState } from "react";
import { Input } from "../../../shared/components/Input";
import { Button, Loader, SuccessActions } from "../../../shared";
import { AppState, appStore } from "../../../core/store";
import { UserDto } from "../../../core/store/slices/userStore";
import { toast } from "react-toastify";
import { useNavigate, useParams } from "react-router-dom";
import { notification$ } from "../../../core/notification";
import { useSubscription } from "observable-hooks";

const getInitialData = (): UserDto => {
  return {
    firstName: "",
    lastName: "",
    email: "",
    password: "",
  };
};

export const UpdateUser = () => {
  const [data, setData] = useState<UserDto>(getInitialData());
  const [confirmPassword, setConfirmPassword] = useState<string>("");
  const userData = appStore((state: AppState) => state.user.userData);
  const fetchUserData = appStore((state: AppState) => state.user.fetchUserData);
  const updateUser = appStore((state: AppState) => state.user.updateUser);
  const params = useParams();
  const navigate = useNavigate();

  const handleSubmit = async (e: { preventDefault: () => void }) => {
    e.preventDefault();
    if (confirmPassword !== data.password) {
      toast.error("Passwords do not match.");
      return;
    }
    try {
      if (data.email === userData.data?.email) {
        data.email = "";
      }
      await updateUser(params.id ?? "", data);
    } catch (err) {}
  };

  useEffect(() => {
    fetchUserData(params.id ?? "");
  }, [fetchUserData, params.id]);

  useEffect(() => {
    setData({
      firstName: userData.data?.firstName ?? "",
      lastName: userData.data?.lastName ?? "",
      email: userData.data?.email ?? "",
      password: "",
    });
  }, [userData]);

  useSubscription(notification$, (notification) => {
    if (notification?.action === SuccessActions.UpdateUserSuccess) {
      navigate("/user");
    }
  });

  const handleReset = () => {
    const data = {
      ...getInitialData(),
      firstName: userData.data?.firstName ?? "",
      lastName: userData.data?.lastName ?? "",
      email: userData.data?.email ?? "",
    };
    setData(data);
    setConfirmPassword("");
  };

  return (
    <div className="max-w-[1000px] mx-auto mt-6">
      <h2 className="pb-5 text-xl">Update User</h2>
      {userData.isLoading && (
        <Loader size="lg" className="flex justify-center pt-10" />
      )}
      {!userData.isLoading && (
        <form onSubmit={handleSubmit}>
          <div className="custom-grid">
            <Input
              label="First Name"
              id="update-first-name"
              value={data.firstName}
              required
              onChange={(e: any) =>
                setData({ ...data, firstName: e.target.value })
              }
            />
            <Input
              label="Last Name"
              id="update-last-name"
              value={data.lastName}
              required
              onChange={(e: any) =>
                setData({ ...data, lastName: e.target.value })
              }
            />
            <Input
              label="Email"
              type="email"
              id="update-email"
              value={data.email}
              onChange={(e: any) => setData({ ...data, email: e.target.value })}
            />
            <div></div>
            <Input
              label="Password"
              type="password"
              id="update-password"
              value={data.password}
              onChange={(e: any) =>
                setData({ ...data, password: e.target.value })
              }
            />
            <Input
              label="Confirm Password"
              type="password"
              id="update-confirm-password"
              value={confirmPassword}
              onChange={(e: any) => setConfirmPassword(e.target.value)}
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
