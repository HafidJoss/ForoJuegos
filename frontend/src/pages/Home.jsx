import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../components/Button';
import FeedReviewItem from '../components/Feed/FeedReviewItem';
import FeedUgcItem from '../components/Feed/FeedUgcItem';
import { getFeed } from '../services/feedService';
import { useAuth } from '../context/AuthContext';

function Home() {
  const [feed, setFeed] = useState([]);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const navigate = useNavigate();
  const { token } = useAuth();

  useEffect(() => {
    loadFeed();
  }, []);

  const loadFeed = async () => {
    if (loading) return;
    setLoading(true);
    try {
      const data = await getFeed({ page, pageSize: 10 });
      if (data.length < 10) {
        setHasMore(false);
      }
      setFeed(prev => {
        const combined = [...prev, ...data];
        const unique = [];
        const seen = new Set();
        for (const item of combined) {
          if (!seen.has(item.id)) {
            seen.add(item.id);
            unique.push(item);
          }
        }
        return unique;
      });
      setPage(prev => prev + 1);
    } catch (error) {
      console.error("Error cargando feed", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page">
      <section className="hero">
        <div>
          <h1>Últimas reseñas y tendencias</h1>
          <p className="muted">
            Descubre lo que la comunidad gamer está jugando y compartiendo ahora mismo.
          </p>
        </div>
        <div className="hero-actions">
          <Button variant="primary" onClick={() => navigate('/games')}>Explorar juegos</Button>
          <Button variant="ghost" onClick={() => navigate('/games')}>Publicar reseña</Button>
        </div>
      </section>

      <section className="feed-section">
        <h2 className="feed-title">Feed de Actividad</h2>
        
        <div className="feed-stream">
          {feed.map(item => {
            if (item.type === 'review') {
              return <FeedReviewItem key={item.id} item={item} token={token} />;
            } else if (item.type === 'ugc') {
              return <FeedUgcItem key={item.id} item={item} token={token} />;
            }
            return null;
          })}
        </div>

        {loading && <div className="feed-loading"><div className="spinner"></div><span>Cargando...</span></div>}
        {!loading && hasMore && (
          <div className="feed-load-more">
            <Button variant="ghost" onClick={loadFeed}>
              Cargar más
            </Button>
          </div>
        )}
        {!hasMore && feed.length > 0 && <p className="feed-end-message">No hay más actividades recientes.</p>}
        {!loading && feed.length === 0 && <p className="feed-end-message">No hay actividad aún. ¡Sé el primero en publicar!</p>}
      </section>
    </div>
  );
}

export default Home;
