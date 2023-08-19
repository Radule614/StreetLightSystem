import { useState } from "react";
import { LoginDto } from "../../core/store/slices/authStore";
import { useNavigate } from "react-router-dom";
import { AppState, appStore } from "../../core/store";
import { Input } from "../../shared/components/Input";
import { Button, Loader } from "../../shared";

const getInitialData = (): LoginDto => {
  return {
    email: "",
    password: "",
  };
};

export const Login = () => {
  const [data, setData] = useState<LoginDto>(getInitialData());
  const login = appStore((state: AppState) => state.auth.login);
  const isLoading = appStore((state: AppState) => state.auth.token.isLoading);
  const navigate = useNavigate();

  const handleSubmit = async (e: { preventDefault: () => void }) => {
    e.preventDefault();

    try {
      await login(data);
      navigate("/");
    } catch (err) {}
  };

  return (
    <div className="max-w-[400px] mx-auto mt-36">
      <form onSubmit={handleSubmit}>
        <div className="custom-grid">
          <Input
            label="Email"
            type="email"
            id="login-email"
            value={data.email}
            onChange={(e: any) => setData({ ...data, email: e.target.value })}
          />
          <Input
            label="Password"
            type="password"
            id="login-password"
            value={data.password}
            onChange={(e: any) =>
              setData({ ...data, password: e.target.value })
            }
          />
        </div>

        <div className="flex gap-3 mt-10 justify-end items-center">
          {isLoading && <Loader size="sm"/>}
          <Button type="submit">Login</Button>
        </div>
      </form>
    </div>
  );
};
