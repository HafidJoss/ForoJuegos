import React, { useState, useEffect } from 'react';
import Card from '../Card';
import Button from '../Button';
import Input from '../Input';
import { listCommentsByUgc, createComment } from '../../services/commentService';
import { likeUgc, dislikeUgc } from '../../services/likeService';

export default function FeedUgcItem({ item, token }) {
  const [showComments, setShowComments] = useState(false);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');
  const [loadingComments, setLoadingComments] = useState(false);
  const [likeCount, setLikeCount] = useState(item.likeCount || 0);
  const [dislikeCount, setDislikeCount] = useState(item.dislikeCount || 0);
  const [likeActive, setLikeActive] = useState(false);
  const [dislikeActive, setDislikeActive] = useState(false);

  const handleLike = async () => {
    if (!token) return;
    try {
      await likeUgc(item.itemId, token);
      if (dislikeActive) {
        setDislikeCount(prev => Math.max(0, prev - 1));
        setDislikeActive(false);
      }
      setLikeCount(prev => prev + 1);
      setLikeActive(true);
    } catch (error) {
      console.error("Error liking UGC", error);
    }
  };

  const handleDislike = async () => {
    if (!token) return;
    try {
      await dislikeUgc(item.itemId, token);
      if (likeActive) {
        setLikeCount(prev => Math.max(0, prev - 1));
        setLikeActive(false);
      }
      setDislikeCount(prev => prev + 1);
      setDislikeActive(true);
    } catch (error) {
      console.error("Error disliking UGC", error);
    }
  };

  useEffect(() => {
    if (showComments && comments.length === 0) {
      loadComments();
    }
  }, [showComments]);

  const loadComments = async () => {
    setLoadingComments(true);
    try {
      const data = await listCommentsByUgc(item.itemId);
      setComments(data);
    } catch (error) {
      console.error("Error loading comments", error);
    } finally {
      setLoadingComments(false);
    }
  };

  const handleAddComment = async (e) => {
    e.preventDefault();
    if (!newComment.trim() || !token) return;

    try {
      const added = await createComment({ ugcId: item.itemId, texto: newComment }, token);
      setComments([added, ...comments]);
      setNewComment('');
    } catch (error) {
      console.error("Error adding comment", error);
    }
  };

  const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

  return (
    <Card className="feed-item ugc-item">
      <div className="feed-item-header">
        <div className="author-info">
          <div className="avatar avatar-lg">{item.authorName.charAt(0).toUpperCase()}</div>
          <div className="meta">
            <strong>{item.authorName}</strong>
            <span className="muted text-sm"> compartió contenido • {new Date(item.date).toLocaleDateString()}</span>
            {item.gameName && <span className="ugc-game-tag">{item.gameName}</span>}
          </div>
        </div>
      </div>
      
      <div className="feed-item-content ugc-content">
        <h3 className="ugc-title">{item.title}</h3>
        {item.content && <p className="ugc-description">{item.content}</p>}
        
        {item.imageUrl && (
          <div className="ugc-image">
            <img src={`${API_BASE}${item.imageUrl}`} alt={item.title} />
          </div>
        )}

        {item.fileUrl && (
          <div className="ugc-file">
            <div className="file-info" style={{ display: 'flex', alignItems: 'center', gap: '8px', flex: 1, minWidth: 0, marginRight: '12px' }}>
              <span style={{ fontSize: '0.75rem', fontWeight: 700, textTransform: 'uppercase', color: 'var(--primary)', background: 'rgba(0, 210, 211, 0.1)', border: '1px solid rgba(0, 210, 211, 0.2)', padding: '3px 8px', borderRadius: '4px', flexShrink: 0, letterSpacing: '0.5px' }}>
                {item.fileName && item.fileName.includes('.') ? item.fileName.split('.').pop().toUpperCase() : 'FILE'}
              </span>
              <span className="file-name" style={{ flex: 1, minWidth: 0, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap', fontSize: '0.85rem' }}>
                {item.fileName}
              </span>
            </div>
            <a href={`${API_BASE}/ugc/download/${item.itemId}`} target="_blank" rel="noopener noreferrer" className="btn btn-primary btn-sm" download style={{ flexShrink: 0 }}>
              Descargar
            </a>
          </div>
        )}
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
