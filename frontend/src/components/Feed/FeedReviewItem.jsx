import React, { useState, useEffect } from 'react';
import Card from '../Card';
import Badge from '../Badge';
import Button from '../Button';
import Input from '../Input';
import { likeReview, dislikeReview } from '../../services/likeService';
import { listCommentsByReview, createComment } from '../../services/commentService';

export default function FeedReviewItem({ item, token }) {
  const [likeCount, setLikeCount] = useState(item.likeCount || 0);
  const [dislikeCount, setDislikeCount] = useState(item.dislikeCount || 0);
  const [showComments, setShowComments] = useState(false);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');
  const [loadingComments, setLoadingComments] = useState(false);
  const [likeActive, setLikeActive] = useState(false);
  const [dislikeActive, setDislikeActive] = useState(false);

  useEffect(() => {
    if (showComments && comments.length === 0) {
      loadComments();
    }
  }, [showComments]);

  const loadComments = async () => {
    setLoadingComments(true);
    try {
      const data = await listCommentsByReview(item.itemId);
      setComments(data);
    } catch (error) {
      console.error("Error loading comments", error);
    } finally {
      setLoadingComments(false);
    }
  };

  const handleLike = async () => {
    if (!token) return;
    try {
      await likeReview(item.itemId, token);
      if (dislikeActive) {
        setDislikeCount(prev => Math.max(0, prev - 1));
        setDislikeActive(false);
      }
      setLikeCount(prev => prev + 1);
      setLikeActive(true);
    } catch (error) {
      console.error("Error liking", error);
    }
  };

  const handleDislike = async () => {
    if (!token) return;
    try {
      await dislikeReview(item.itemId, token);
      if (likeActive) {
        setLikeCount(prev => Math.max(0, prev - 1));
        setLikeActive(false);
      }
      setDislikeCount(prev => prev + 1);
      setDislikeActive(true);
    } catch (error) {
      console.error("Error disliking", error);
    }
  };

  const handleAddComment = async (e) => {
    e.preventDefault();
    if (!newComment.trim() || !token) return;
    try {
      const added = await createComment({ resenaId: item.itemId, texto: newComment }, token);
      setComments([added, ...comments]);
      setNewComment('');
    } catch (error) {
      console.error("Error adding comment", error);
    }
  };

  const renderStars = (rating) => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(
        <span key={i} className={`star ${i <= rating ? 'star-filled' : 'star-empty'}`}>★</span>
      );
    }
    return <div className="star-rating">{stars}</div>;
  };

  return (
    <Card className="feed-item review-item">
      <div className="feed-item-header">
        <div className="author-info">
          <div className="avatar">{item.authorName.charAt(0).toUpperCase()}</div>
          <div className="meta">
            <strong>{item.authorName}</strong>
            <span className="muted text-sm"> reseñó un juego • {new Date(item.date).toLocaleDateString()}</span>
          </div>
        </div>
      </div>

      <div className="feed-item-content review-content">
        <div className="review-game-header">
          <span className="review-game-name">{item.gameName}</span>
          {item.rating && renderStars(item.rating)}
        </div>
        <p className="review-text">{item.content}</p>
      </div>

      <div className="feed-item-footer">
        <button
          className={`reaction-btn ${likeActive ? 'active-like' : ''}`}
          onClick={handleLike}
          disabled={!token}
          title={token ? 'Me gusta' : 'Inicia sesión para dar like'}
          style={{ fontSize: '0.85rem', fontWeight: 600 }}
        >
          <span>Likes ({likeCount})</span>
        </button>
        <button
          className={`reaction-btn ${dislikeActive ? 'active-dislike' : ''}`}
          onClick={handleDislike}
          disabled={!token}
          title={token ? 'No me gusta' : 'Inicia sesión para dar dislike'}
          style={{ fontSize: '0.85rem', fontWeight: 600 }}
        >
          <span>Dislikes ({dislikeCount})</span>
        </button>
        <button
          className="reaction-btn"
          onClick={() => setShowComments(!showComments)}
          style={{ fontSize: '0.85rem', fontWeight: 600 }}
        >
          <span>Comentarios ({comments.length > 0 ? comments.length : (item.commentCount || 0)})</span>
        </button>
      </div>

      {showComments && (
        <div className="comments-section">
          {token ? (
            <form onSubmit={handleAddComment} className="comment-form">
              <textarea
                value={newComment}
                onChange={(e) => setNewComment(e.target.value)}
                placeholder="Escribe un comentario..."
                required
              />
              <div className="comment-submit-row">
                <Button type="submit">Enviar</Button>
              </div>
            </form>
          ) : (
            <p className="muted text-sm">Inicia sesión para comentar.</p>
          )}

          {loadingComments ? (
            <p>Cargando comentarios...</p>
          ) : (
            <div className="comments-list">
              {comments.map(c => (
                <div key={c.id} className="comment">
                  <div className="comment-author">
                    <strong>{c.usuarioNombre || 'Anónimo'}</strong> <span className="muted text-xs">{new Date(c.fechaCreacion).toLocaleString()}</span>
                  </div>
                  <p>{c.texto}</p>
                </div>
              ))}
              {comments.length === 0 && <p className="muted text-sm">Sé el primero en comentar.</p>}
            </div>
          )}
        </div>
      )}
    </Card>
  );
}
