
package com.portal.admin.controller;

import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/webhooks/pix")
public class PixWebhookController {

  @PostMapping
  public void confirmarPagamento() {
    // atualizar pedido para PAGO
  }
}
