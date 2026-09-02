import { useState, useRef, useEffect } from 'react';
import { useCart } from '../context/CartContext';
import { sendChatMessage } from '../services/api';
import toast from 'react-hot-toast';
import '../styles/ChatAssistant.css';

const ChatAssistant = () => {
    const { cartItems, addToCart } = useCart();
    const [isOpen, setIsOpen] = useState(false);
    const [messages, setMessages] = useState([
        {
            id: 1,
            text: '¡Hola! Soy tu asistente virtual de Firmeza. 🏗️ Te ayudo con recomendaciones de productos, información de stock, precios y el proceso de compra. ¿En qué te puedo asesorar hoy?',
            sender: 'bot',
            timestamp: new Date()
        }
    ]);
    const [inputMessage, setInputMessage] = useState('');
    const [isTyping, setIsTyping] = useState(false);
    const messagesEndRef = useRef(null);
    const inputRef = useRef(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages, isTyping]);

    useEffect(() => {
        if (isOpen && inputRef.current) {
            inputRef.current.focus();
        }
    }, [isOpen]);

    const handleSendMessage = async (e, textOverride = null) => {
        if (e) e.preventDefault();

        const messageText = textOverride || inputMessage;
        if (!messageText.trim()) return;

        const userMessage = {
            id: Date.now(),
            text: messageText,
            sender: 'user',
            timestamp: new Date()
        };

        setMessages(prev => [...prev, userMessage]);
        if (!textOverride) setInputMessage('');
        setIsTyping(true);

        try {
            // Generar el contexto del carrito para la API de Gemini
            const cartContext = cartItems.length > 0 
                ? cartItems.map(i => `${i.quantity}x ${i.nombre} ($${i.precio})`).join(', ')
                : 'Carrito vacío';

            const data = await sendChatMessage(messageText, cartContext);

            const botMessage = {
                id: Date.now() + 1,
                text: data.response || 'Lo siento, no pude procesar tu mensaje.',
                sender: 'bot',
                products: data.products || null,
                timestamp: new Date()
            };

            setMessages(prev => [...prev, botMessage]);
        } catch (error) {
            console.error('Error al enviar mensaje al chat:', error);
            const errorMessage = {
                id: Date.now() + 1,
                text: 'Hubo un inconveniente técnico conectando con mi cerebro IA. Puedes consultar el catálogo mientras me recupero.',
                sender: 'bot',
                timestamp: new Date()
            };
            setMessages(prev => [...prev, errorMessage]);
        } finally {
            setIsTyping(false);
        }
    };

    const quickQuestions = [
        '🧱 ¿Qué tipo de cemento o concreto tienen?',
        '💳 ¿Cuáles son los métodos de pago aceptados?',
        '🚚 ¿Hacen envíos a domicilio y cuánto tarda?',
        '🛒 ¿Qué productos tengo en mi carrito?'
    ];

    const handleQuickQuestion = (question) => {
        handleSendMessage(null, question);
    };

    const handleAddProductFromChat = (product) => {
        addToCart(product);
        toast.success(`Añadido ${product.nombre} desde el chat!`, {
            icon: '🛒',
            style: {
                borderRadius: '12px',
                background: 'rgba(23, 32, 52, 0.95)',
                color: '#f1f5f9',
            }
        });
    };

    const handleClearChat = () => {
        setMessages([
            {
                id: Date.now(),
                text: 'Chat reiniciado. ¿En qué más te puedo colaborar?',
                sender: 'bot',
                timestamp: new Date()
            }
        ]);
    };

    const renderFormattedText = (text) => {
        // Formato simple de negritas y saltos de línea
        const parts = text.split(/(\*\*.*?\*\*)/g);
        return parts.map((part, index) => {
            if (part.startsWith('**') && part.endsWith('**')) {
                return <strong key={index}>{part.slice(2, -2)}</strong>;
            }
            return part;
        });
    };

    return (
        <>
            {/* Chat Trigger Button */}
            <button
                className={`chat-assistant-button ${isOpen ? 'active' : ''}`}
                onClick={() => setIsOpen(!isOpen)}
                aria-label="Asistente virtual de compras"
                id="chat-assistant-trigger"
            >
                {isOpen ? (
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                        <line x1="18" y1="6" x2="6" y2="18"></line>
                        <line x1="6" y1="6" x2="18" y2="18"></line>
                    </svg>
                ) : (
                    <svg xmlns="http://www.w3.org/2000/svg" width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                        <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
                        <path d="M8 10h.01"></path>
                        <path d="M12 10h.01"></path>
                        <path d="M16 10h.01"></path>
                    </svg>
                )}
                {!isOpen && <span className="chat-notification-badge">IA</span>}
            </button>

            {/* Chat Modal Window */}
            <div className={`chat-assistant-window ${isOpen ? 'open' : ''}`} id="chat-assistant-window">
                {/* Header */}
                <div className="chat-header">
                    <div className="chat-header-info">
                        <div className="chat-avatar">
                            <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <path d="M12 2a10 10 0 1 0 10 10A10 10 0 0 0 12 2z"></path>
                                <path d="M12 6v6l4 2"></path>
                            </svg>
                        </div>
                        <div>
                            <h3>Asistente IA Firmeza</h3>
                            <span className="chat-status">
                                <span className="status-dot"></span>
                                Catálogo en tiempo real
                            </span>
                        </div>
                    </div>

                    <div className="chat-header-actions">
                        <button
                            className="chat-action-btn"
                            onClick={handleClearChat}
                            title="Limpiar conversación"
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <polyline points="1 4 1 10 7 10"></polyline>
                                <path d="M3.51 15a9 9 0 1 0 2.13-9.36L1 10"></path>
                            </svg>
                        </button>
                        <button
                            className="chat-action-btn"
                            onClick={() => setIsOpen(false)}
                            aria-label="Cerrar chat"
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <line x1="18" y1="6" x2="6" y2="18"></line>
                                <line x1="6" y1="6" x2="18" y2="18"></line>
                            </svg>
                        </button>
                    </div>
                </div>

                {/* Messages Body */}
                <div className="chat-messages">
                    {messages.map((msg) => (
                        <div
                            key={msg.id}
                            className={`chat-message ${msg.sender === 'user' ? 'user-message' : 'bot-message'}`}
                        >
                            {msg.sender === 'bot' && (
                                <div className="message-avatar">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                                        <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                                    </svg>
                                </div>
                            )}

                            <div className="message-content">
                                <p>{renderFormattedText(msg.text)}</p>

                                {/* Recommended Product Card inside Chat */}
                                {msg.products && msg.products.length > 0 && (
                                    <div className="chat-product-cards">
                                        {msg.products.map(product => (
                                            <div key={product.id} className="chat-product-card">
                                                <div className="chat-product-details">
                                                    <span className="chat-product-name">{product.nombre}</span>
                                                    <span className="chat-product-price">${product.precio?.toLocaleString('es-CO')}</span>
                                                </div>
                                                <button
                                                    className="chat-add-btn"
                                                    onClick={() => handleAddProductFromChat(product)}
                                                >
                                                    + Añadir
                                                </button>
                                            </div>
                                        ))}
                                    </div>
                                )}

                                <span className="message-time">
                                    {msg.timestamp.toLocaleTimeString('es-ES', {
                                        hour: '2-digit',
                                        minute: '2-digit'
                                    })}
                                </span>
                            </div>
                        </div>
                    ))}

                    {/* Typing Indicator */}
                    {isTyping && (
                        <div className="chat-message bot-message">
                            <div className="message-avatar">
                                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect>
                                    <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                                </svg>
                            </div>
                            <div className="message-content typing-indicator">
                                <span></span>
                                <span></span>
                                <span></span>
                            </div>
                        </div>
                    )}

                    <div ref={messagesEndRef} />
                </div>

                {/* Quick Questions Suggestions */}
                {messages.length <= 3 && (
                    <div className="chat-quick-questions">
                        <p className="quick-questions-title">Preguntas sugeridas:</p>
                        <div className="quick-questions-grid">
                            {quickQuestions.map((q, idx) => (
                                <button
                                    key={idx}
                                    className="quick-question-button"
                                    onClick={() => handleQuickQuestion(q)}
                                >
                                    {q}
                                </button>
                            ))}
                        </div>
                    </div>
                )}

                {/* Input Area */}
                <form className="chat-input-container" onSubmit={handleSendMessage}>
                    <input
                        ref={inputRef}
                        type="text"
                        className="chat-input"
                        placeholder="Escribe tu consulta sobre productos..."
                        value={inputMessage}
                        onChange={(e) => setInputMessage(e.target.value)}
                        disabled={isTyping}
                    />
                    <button
                        type="submit"
                        className="chat-send-button"
                        disabled={!inputMessage.trim() || isTyping}
                        aria-label="Enviar consulta"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <line x1="22" y1="2" x2="11" y2="13"></line>
                            <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
                        </svg>
                    </button>
                </form>
            </div>
        </>
    );
};

export default ChatAssistant;
