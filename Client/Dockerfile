# ===================== BUILD STAGE =====================
FROM node:22 AS build
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm ci

# Copy the rest of the client app
COPY . .

# Build the Vite app (output: /app/dist)
RUN npm run build

# ===================== RUNTIME STAGE =====================
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Remove default nginx files
RUN rm -rf ./*

# Copy built static files from build stage
COPY --from=build /app/dist ./

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
