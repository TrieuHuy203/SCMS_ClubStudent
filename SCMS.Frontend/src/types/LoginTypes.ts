export interface LoginRequest {
  UsernameOrEmail: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: string;
  // Thêm các trường khác nếu backend trả về
}
