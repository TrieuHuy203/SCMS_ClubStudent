import React, { useState } from "react";

interface ClubRegisterFormProps {
  clubId: string;
  onSubmitSuccess?: () => void;
}

interface RegisterFormData {
  RegisterReason: string;
  Skills: string;
  Experience: string;
  DesiredRole: string;
}

const ClubRegisterForm: React.FC<ClubRegisterFormProps> = ({ clubId, onSubmitSuccess }) => {
  const [form, setForm] = useState<RegisterFormData>({
    RegisterReason: "",
    Skills: "",
    Experience: "",
    DesiredRole: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      // Lấy token từ localStorage hoặc context
      const token = localStorage.getItem("token");
      const response = await fetch("http://localhost:5217/api/user/Membership/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ ClubId: clubId, ...form }),
      });
      if (!response.ok) throw new Error("Đăng ký thất bại");
      alert("Đăng ký thành công!");
      if (onSubmitSuccess) onSubmitSuccess();
    } catch (err: any) {
      setError(err.message || "Đăng ký thất bại");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h3>Đăng ký thành viên CLB</h3>
      <div>
        <label>Lý do đăng ký:</label>
        <textarea name="RegisterReason" value={form.RegisterReason} onChange={handleChange} required />
      </div>
      <div>
        <label>Kỹ năng:</label>
        <input name="Skills" value={form.Skills} onChange={handleChange} required />
      </div>
      <div>
        <label>Kinh nghiệm:</label>
        <input name="Experience" value={form.Experience} onChange={handleChange} required />
      </div>
      <div>
        <label>Vai trò mong muốn:</label>
        <input name="DesiredRole" value={form.DesiredRole} onChange={handleChange} required />
      </div>
      {error && <div style={{ color: "red" }}>{error}</div>}
      <button type="submit" disabled={loading}>{loading ? "Đang gửi..." : "Đăng ký"}</button>
    </form>
  );
};

export default ClubRegisterForm;