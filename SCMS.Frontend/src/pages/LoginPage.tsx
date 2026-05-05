import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../services/authService";
import { LoginRequest } from "../types/LoginTypes";

const LoginPage: React.FC = () => {
  const [form, setForm] = useState<LoginRequest>({
    UsernameOrEmail: "",
    password: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const navigate = useNavigate();
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const res = await login(form);
      alert("Đăng nhập thành công! Token: " + res.token);
      // Lưu token nếu cần
      localStorage.setItem("token", res.token);
      // Chuyển sang trang chủ user
      navigate("/user/home");
    } catch (err: any) {
      setError(err.message || "Đăng nhập thất bại");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Đăng nhập</h2>
      <div>
        <label>Tên đăng nhập hoặc Email:</label>
        <input
          name="UsernameOrEmail"
          type="text"
          value={form.UsernameOrEmail}
          onChange={handleChange}
          required
        />
      </div>
      <div>
        <label>Mật khẩu:</label>
        <input
          name="password"
          type="password"
          value={form.password}
          onChange={handleChange}
          required
        />
      </div>
      {error && <div style={{ color: "red" }}>{error}</div>}
      <button type="submit" disabled={loading}>
        {loading ? "Đang đăng nhập..." : "Đăng nhập"}
      </button>
    </form>
  );
};

export default LoginPage;
