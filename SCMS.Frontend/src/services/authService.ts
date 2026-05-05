import { LoginRequest, LoginResponse } from "../types/LoginTypes";

export async function login(data: LoginRequest): Promise<LoginResponse> {
  // Thay URL này bằng endpoint thực tế của bạn
  const response = await fetch("http://localhost:5217/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    throw new Error("Đăng nhập thất bại");
  }

  return response.json();
}