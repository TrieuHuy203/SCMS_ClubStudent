import { Club } from "../types/ClubTypes";

export async function getClubs(): Promise<Club[]> {
  const response = await fetch("http://localhost:5217/api/admin/club"); // Đổi URL nếu cần
  if (!response.ok) {
    throw new Error("Không thể lấy danh sách câu lạc bộ");
  }
  return response.json();
}
