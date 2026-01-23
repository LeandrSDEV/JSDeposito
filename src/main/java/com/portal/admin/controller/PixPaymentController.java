
package com.portal.admin.controller;

import org.springframework.web.bind.annotation.*;
import java.time.LocalDateTime;
import java.util.Map;

@RestController
@RequestMapping("/api/payments")
public class PixPaymentController {

  @PostMapping("/pix")
  public Map<String, Object> gerarPix() {
    return Map.of(
      "pixKey", "79988012359",
      "qrCode", "BASE64_QRCODE_AQUI",
      "expiresAt", LocalDateTime.now().plusSeconds(60)
    );
  }

  @PostMapping("/cancel/{pedidoId}")
  public void cancelar(@PathVariable Long pedidoId) {}

  @GetMapping("/status/{pedidoId}")
  public String status(@PathVariable Long pedidoId) {
    return "PENDENTE";
  }
}
