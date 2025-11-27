# Stage 1: Build the React application
FROM node:20-alpine AS build-stage
WORKDIR /app
COPY Firmeza.Client/package*.json ./
RUN npm install
COPY Firmeza.Client/. .
RUN npm run build

# Stage 2: Serve the React application with Nginx
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
