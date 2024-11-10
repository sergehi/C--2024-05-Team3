import ProtectedRoute from "@/components/protected-route";
import { Navbar } from "@/components/navbar";

export default function Home() {
  return (
    <ProtectedRoute>
      <div className="dark:bg-gray-950 min-h-screen">
        <Navbar />
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            height: "calc(100vh - 80px)",
            gap: "40px",
          }}
        >
          <button className="py-2 w-1/4 h-1/2 text-2xl dark:bg-cyan-700 hover:opacity-80 rounded-3xl shadow-2xl focus:outline-none focus:ring transition-opacity duration-150 ease-in-out">
            Создать организацию
          </button>

          <button className="py-2 w-1/4 h-1/2 text-2xl dark:bg-gray-700 hover:opacity-80 rounded-3xl shadow-2xl focus:outline-none focus:ring transition-opacity duration-150 ease-in-out">
            Вступить по приглашению
          </button>
        </div>
      </div>
    </ProtectedRoute>
  );
}
