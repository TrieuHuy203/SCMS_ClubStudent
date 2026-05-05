import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getClubs } from "../services/clubService";
import { Club } from "../types/ClubTypes";

const ClubListPage: React.FC = () => {
  const [clubs, setClubs] = useState<Club[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getClubs()
      .then(setClubs)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  const navigate = useNavigate();
  const handleRegister = (clubId: string) => {
    navigate(`/clubs/${clubId}/register`);
  };

  if (loading) return <div>Đang tải...</div>;
  if (error) return <div style={{ color: "red" }}>{error}</div>;

  return (
    <div>
      <h2>Danh sách câu lạc bộ</h2>
      <ul>
        {clubs.map((club) => (
          <li key={club.id} style={{ marginBottom: "12px" }}>
            <strong>{club.name}</strong>
            <div>{club.description}</div>
            <button onClick={() => handleRegister(club.id)}>Đăng ký</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ClubListPage;
